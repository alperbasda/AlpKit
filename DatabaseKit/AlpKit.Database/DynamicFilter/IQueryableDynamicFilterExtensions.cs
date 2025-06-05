using AlpKit.Database.DynamicFilter.Models;
using AlpKit.Helpers.EnumExtensions;
using System.Linq.Dynamic.Core;
using System.Text;

namespace AlpKit.Database.DynamicFilter;

public static class IQueryableDynamicFilterExtensions
{
    private static readonly IDictionary<FilterOperator, string> _operators = new Dictionary<FilterOperator, string>
    {
        { FilterOperator.Equals, "=" },
        { FilterOperator.DoesntEqual, "!=" },
        { FilterOperator.LessThan, "<" },
        { FilterOperator.LessThanOrEqual, "<=" },
        { FilterOperator.GreaterThan, ">" },
        { FilterOperator.GreaterThanOrEqual, ">=" },
        { FilterOperator.IsEmpty, "== null" },
        { FilterOperator.IsNotEmpty, "!= null" },
        { FilterOperator.StartsWith, "StartsWith" },
        { FilterOperator.EndsWith, "EndsWith" },
        { FilterOperator.Contains, "Contains" },
        { FilterOperator.ContainsIgnoreCase, "Contains" },
        { FilterOperator.NotContains, "DoesNotContains" }
    };

    public static Filter GetNewWay(Filter filter, Logic logic)
    {
        if (filter.Filters == null)
            filter.Filters = new List<Filter>();

        var newWay = new Filter { Field = string.Empty, Logic = logic, Operator = FilterOperator.Equals, Value = "1", Filters = new List<Filter>() };
        filter.Filters.Add(newWay);
        return newWay;
    }

    public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery? dynamicQuery)
    {
        if (dynamicQuery is null)
            return query;
        if (dynamicQuery.Filter is not null)
            query = Filter(query, dynamicQuery.Filter);
        if (dynamicQuery.Sort is not null)
            query = Sort(query, dynamicQuery.Sort);
        return query;
    }

    private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
    {
        IList<Filter> filters = GetAllFilters(filter);
        string?[] values = filters.Select(f => IsStringOperator(_operators[f.Operator]) ? f.Value.ToLower() : f.Value).ToArray();
        string where = Transform(filter, filters);
        if (!string.IsNullOrEmpty(where) && values != null)
            queryable = queryable.Where(where, values);

        return queryable;
    }

    private static IQueryable<T> Sort<T>(IQueryable<T> queryable, Sort sort)
    {
        if (string.IsNullOrEmpty(sort.Field))
            throw new ArgumentException("Invalid Field");

        if (sort is not null)
        {
            return queryable.OrderBy($"{sort.Field} {sort.OrderOperator.GetDescription()}");
        }

        return queryable;
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

    public static string Transform(Filter filter, IList<Filter> filters)
    {
        int index = filters.IndexOf(filter);
        StringBuilder where = new();

        if (filter.Operator == FilterOperator.Raw && !string.IsNullOrEmpty(filter.Field))
        {
            return filter.Field.Replace("@value", $"@{index}");
        }

        string comparison = _operators[filter.Operator];

        if (!string.IsNullOrEmpty(filter.Value))
        {
            if (filter.Operator == FilterOperator.NotContains)
                where.Append($"(!np({filter.Field}).{comparison}(@{index}))");
            else if (comparison is "StartsWith" or "EndsWith" or "Contains")
                where.Append($"(np({filter.Field}).{comparison}(@{index}))");
            else if (comparison is "ContainsIgnoreCase")
                where.Append($"(np({filter.Field}).ToLower().Contains(@{index}))");
            else
                where.Append($"np({filter.Field}) {comparison} @{index}");
        }
        else if (filter.Operator is FilterOperator.IsEmpty or FilterOperator.IsNotEmpty)
        {
            where.Append($"np({filter.Field}) {comparison}");
        }

        if (filter.Logic is not null && filter.Filters is not null && filter.Filters.Any())
        {
            if (string.IsNullOrEmpty(filter.Field))
            {
                return $"({string.Join($" {filter.Logic.Value.GetDescription()} ", filter.Filters.Select(f => Transform(f, filters)))})";
            }

            return $"{where} {filter.Logic} ({string.Join($" {filter.Logic.Value.GetDescription()} ", filter.Filters.Select(f => Transform(f, filters)))})";
        }

        return where.ToString();
    }

    private static bool IsStringOperator(string comparison)
    {
        return comparison is "StartsWith" or "EndsWith" or "Contains";
    }
}
