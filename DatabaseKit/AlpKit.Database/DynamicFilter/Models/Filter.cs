namespace AlpKit.Database.DynamicFilter.Models;

public class Filter : ICloneable
{
    public string Field { get; set; }
    public string? Value { get; set; }
    public FilterOperator Operator { get; set; }
    public Logic? Logic { get; set; }

    public List<Filter>? Filters { get; set; }

    public Filter()
    {
        Field = string.Empty;
        Operator = FilterOperator.Unknown;
    }

    public Filter(string field, FilterOperator @operator)
    {
        Field = field;
        Operator = @operator;
    }

    public static Filter Create(string prop, FilterOperator filterOperator, string value)
    {
        return new Filter
        {
            Field = prop,
            Value = value,
            Operator = filterOperator
        };
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
