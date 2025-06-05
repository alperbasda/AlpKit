using AlpKit.Database.DynamicFilter.Models;
using System.Collections.Specialized;
using System.Reflection;

namespace AlpKit.Database.NameValueCollectionHelpers;

public static class CollectionToDynamicQueryExtension
{
    public const string SortFieldQueryStringName = "Sort.Field";

    public const string SortOrderOperatorQueryStringName = "Sort.OrderOperator";

    private static string[] ExtractFilters = new[] { CollectionToPageRequestExtension.PageQueryStringName, CollectionToPageRequestExtension.PageSizeQueryStringName, SortOrderOperatorQueryStringName, SortFieldQueryStringName };

    public static DynamicQuery ToDynamicFilter<T>(this NameValueCollection nvc)
    {
        DynamicQuery dynamicQuery = new DynamicQuery
        {
            Sort = GetSortInfo(nvc),
            Filter = BuildDynamicFilters<T>(nvc)
        };

        return dynamicQuery;
    }

    private static Sort? GetSortInfo(NameValueCollection nvc)
    {
        bool hasSort = false;
        var sort = new Sort();

        if (!string.IsNullOrEmpty(nvc[SortFieldQueryStringName]))
        {
            sort.Field = nvc[SortFieldQueryStringName]!;
            hasSort = true;
        }

        if (!string.IsNullOrEmpty(nvc[SortOrderOperatorQueryStringName]))
        {
            sort.OrderOperator = nvc[SortOrderOperatorQueryStringName]!.ToLower() == "asc" ? OrderOperator.Asc : OrderOperator.Desc;
        }

        return hasSort ? sort : null;
    }

    private static Filter? BuildDynamicFilters<T>(NameValueCollection nvc)
    {

        Filter? filter = null;
        PropertyInfo[] properties = typeof(T).GetProperties();
        List<string> allKeys = nvc.AllKeys.Where(w => !ExtractFilters.Contains(w) && w != null).ToList() as List<string>;

        foreach (string key in allKeys)
        {
            var maybeExists = key.Contains('.');
            PropertyInfo? propertyInfo = properties.FirstOrDefault((w) => w.Name == key);


            if (maybeExists)
            {
                if (key.StartsWith("Translations.", StringComparison.OrdinalIgnoreCase))
                {
                    var field = key.Split('.')[1];
                    var langCode = Thread.CurrentThread.CurrentCulture.Name;
                    var raw = $"Translations.Any(LanguageId == \"{langCode}\" && {field}.ToLower().Contains(@value))";

                    filter = AddOrCreateFilter(filter, new Filter
                    {
                        Field = raw,
                        Value = nvc[key],
                        Operator = FilterOperator.Raw
                    });
                }

                filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.Equals, nvc[key]!));
                continue;
            }
            if (propertyInfo == null || string.IsNullOrEmpty(nvc[key]))
                continue;



            if (propertyInfo.PropertyType.IsEnum)
            {
                filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.Equals, nvc[key]!));
                continue;
            }

            switch (Type.GetTypeCode(propertyInfo.PropertyType))
            {
                case TypeCode.DateTime:
                    {
                        filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.GreaterThanOrEqual, nvc[key]!));
                        if (DateTime.TryParse(nvc[key], out var result3))
                        {
                            filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.LessThanOrEqual, result3.Date.AddHours(23.0).AddMinutes(59.0).AddSeconds(59.0).ToString("yyyy-MM-dd HH:mm:ss")));
                        }

                        break;
                    }
                case TypeCode.String:
                    filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.Contains, nvc[key]!));
                    break;
                default:
                    if (propertyInfo.PropertyType.IsNullableType())
                    {
                        switch (Type.GetTypeCode(propertyInfo.PropertyType.GenericTypeArguments[0]))
                        {
                            case TypeCode.DateTime:
                                {
                                    filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.GreaterThanOrEqual, nvc[key]!));
                                    if (DateTime.TryParse(nvc[key], out var result3))
                                    {
                                        filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.LessThanOrEqual, result3.Date.AddHours(23.0).AddMinutes(59.0).AddSeconds(59.0).ToString("yyyy-MM-dd HH:mm:ss")));
                                    }

                                    break;
                                }
                            case TypeCode.String:
                                filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.Contains, nvc[key]!));
                                break;
                            default:
                                if (propertyInfo.PropertyType.IsNullableType())
                                {

                                }
                                filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.Equals, nvc[key]!));
                                break;
                        }
                    }
                    else
                    {
                        filter = AddOrCreateFilter(filter, Filter.Create(key, FilterOperator.Equals, nvc[key]!));
                    }

                    break;
            }
        }

        return filter;

    }

    private static Filter AddOrCreateFilter(Filter? baseFilter, Filter assignFilter)
    {
        assignFilter = CreateSubFilters(assignFilter);
        if (baseFilter == null)
        {
            return assignFilter;
        }
        if (baseFilter.Filters == null)
        {
            baseFilter.Filters = new List<Filter>();
        }

        baseFilter.Filters.Add(assignFilter);

        return baseFilter;
    }

    private static Filter CreateSubFilters(Filter filter)
    {
        var splittedValues = filter.Value!.Split('|');
        if (splittedValues.Length == 1)
        {
            filter.Logic = Logic.And;
            return filter;
        }

        filter.Logic = Logic.Or;


        var newFilter = new Filter()
        {
            Field = filter.Field,
            Logic = Logic.Or,
            Operator = filter.Operator,
            Value = splittedValues[0],
            Filters = new List<Filter>(),
        };

        for (int i = 1; i < splittedValues.Length; i++)
        {
            newFilter.Filters.Add(new Filter
            {
                Field = filter.Field,
                Value = splittedValues[i],
                Operator = filter.Operator,
                Logic = Logic.Or,
            });
        }
        return newFilter;

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
