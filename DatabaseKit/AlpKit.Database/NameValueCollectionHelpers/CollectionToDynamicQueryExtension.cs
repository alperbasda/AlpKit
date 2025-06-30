using AlpKit.Database.DynamicFilter.Models;
using AlpKit.Database.NameValueCollectionHelpers;
using System.Collections.Specialized;
using System.Reflection;

public static class CollectionToDynamicQueryExtension
{
    public const string SortFieldQueryStringName = "Sort.Field";

    public const string SortOrderOperatorQueryStringName = "Sort.OrderOperator";

    public const string LanguageCodeFilterName = "LanguageCode";

    private static string[] ExtractFilters = new[] { CollectionToPageRequestExtension.PageQueryStringName, CollectionToPageRequestExtension.PageSizeQueryStringName, SortOrderOperatorQueryStringName, SortFieldQueryStringName, LanguageCodeFilterName };

    public static DynamicQuery ToDynamicFilter<T>(this NameValueCollection nvc)
    {
        DynamicQuery dynamicQuery = new DynamicQuery
        {
            Sort = GetSortInfo(nvc),
            Filter = BuildDynamicFilters<T>(nvc),
            LanguageCode = nvc[LanguageCodeFilterName] ?? Thread.CurrentThread.CurrentCulture.Name
        };

        return dynamicQuery;
    }

    private static Sort? GetSortInfo(NameValueCollection nvc)
    {
        bool hasSort = false;
        var sort = new Sort();

        if (!string.IsNullOrEmpty(nvc[SortFieldQueryStringName]))
        {
            var rawField = nvc[SortFieldQueryStringName]!;
            hasSort = true;

            if (rawField.StartsWith("Translations.", StringComparison.OrdinalIgnoreCase))
            {
                var fieldName = rawField.Split('.')[1];
                var langCode = nvc[LanguageCodeFilterName] ?? Thread.CurrentThread.CurrentCulture.Name;

                sort.Field = $"Translations.First(LanguageCode == \"{langCode}\").{fieldName}";
            }
            else
            {
                sort.Field = rawField;
            }
        }

        if (!string.IsNullOrEmpty(nvc[SortOrderOperatorQueryStringName]))
        {
            sort.OrderOperator = nvc[SortOrderOperatorQueryStringName]!.ToLower() == "asc"
                ? OrderOperator.Asc
                : OrderOperator.Desc;
        }

        return hasSort ? sort : null;
    }


    private static Filter? BuildDynamicFilters<T>(NameValueCollection nvc)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();

        List<string> allKeys = nvc.AllKeys
            .Where(w => !ExtractFilters.Contains(w) && !string.IsNullOrWhiteSpace(w) && !string.IsNullOrWhiteSpace(nvc[w]))
            .ToList()!;

        Filter? rootFilter = null;

        foreach (string key in allKeys)
        {

            string value = nvc[key]!;
            bool isTranslation = key.StartsWith("Translations.", StringComparison.OrdinalIgnoreCase);
            Filter newFilter;

            if (isTranslation)
            {
                var field = key.Split('.')[1];
                var langCode = nvc[LanguageCodeFilterName] ?? Thread.CurrentThread.CurrentCulture.Name;
                var raw = $"Translations.Any(LanguageCode == \"{langCode}\" && {field}.ToLower().Contains(@value))";

                newFilter = CreateSubFilters(new Filter
                {
                    Field = raw,
                    Value = value,
                    Operator = FilterOperator.Raw
                });
            }
            else
            {
                var property = properties.FirstOrDefault(w => w.Name == key);
                var op = FilterOperator.Equals;



                if (property == null)
                {
                    newFilter = CreateSubFilters(Filter.Create(key, op, value));
                }
                else
                {
                    Type propType = property.PropertyType;
                    if (propType.IsNullableType())
                        propType = Nullable.GetUnderlyingType(propType)!;

                    switch (Type.GetTypeCode(propType))
                    {
                        case TypeCode.String:
                            op = FilterOperator.Contains;
                            break;
                        case TypeCode.DateTime:
                            var filters = new List<Filter>
                        {
                            Filter.Create(key, FilterOperator.GreaterThanOrEqual, value)
                        };

                            if (DateTime.TryParse(value, out var dt))
                            {
                                filters.Add(Filter.Create(key, FilterOperator.LessThanOrEqual,
                                    dt.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-dd HH:mm:ss")));
                            }

                            foreach (var f in filters)
                                rootFilter = AppendFilter(rootFilter, CreateSubFilters(f));

                            continue;

                        default:
                            op = FilterOperator.Equals;
                            break;
                    }

                    newFilter = CreateSubFilters(Filter.Create(key, op, value));
                }
            }

            rootFilter = AppendFilter(rootFilter, newFilter);
        }

        return rootFilter;
    }


    private static Filter CreateSubFilters(Filter filter)
    {
        var values = filter.Value!.Split('|', StringSplitOptions.RemoveEmptyEntries);
        if (values.Length <= 1)
            return filter;

        var first = values.First();
        var root = new Filter
        {
            Field = filter.Field,
            Operator = filter.Operator,
            Logic = Logic.Or,
            Value = first,
            Filters = [.. values.Where(w => w != first).Select(v => new Filter
            {
                Field = filter.Field,
                Operator = filter.Operator,
                Value = v,
                Logic = Logic.Or
            })]
        };

        return root;
    }

    private static Filter AppendFilter(Filter? root, Filter newFilter)
    {
        if (root == null)
            return newFilter;

        if (root.Filters == null)
            root.Filters = new List<Filter>();

        root.Logic = Logic.And;
        root.Filters.Add(newFilter);
        return root;
    }


    public static IList<Filter> GetAllFilters(Filter filter)
    {
        List<Filter> filters = new();
        GetFilters(filter, filters);
        return filters;
    }

    private static void GetFilters(Filter filter, IList<Filter> filters)
    {
        filters.Add(filter);
        if (filter.Filters is not null && filter.Filters.Any())
            foreach (Filter item in filter.Filters)
                GetFilters(item, filters);
    }


    public static bool IsGenericType(this Type type, Type genericType) => type.IsGenericType && type.GetGenericTypeDefinition() == genericType;

    public static bool IsNullableType(this Type type) => type.IsGenericType(typeof(Nullable<>));
}