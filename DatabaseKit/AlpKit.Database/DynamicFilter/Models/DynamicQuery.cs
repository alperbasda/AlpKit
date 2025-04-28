namespace AlpKit.Database.DynamicFilter.Models;

public class DynamicQuery : ICloneable
{
    public Sort? Sort { get; set; }
    public Filter? Filter { get; set; }

    public DynamicQuery()
    {

    }

    public DynamicQuery(Filter? filter, Sort? sort = null)
    {
        Filter = filter;
        Sort = sort;
    }

    public static DynamicQuery Create(Filter? filter, Sort? sort = null)
    {
        return new DynamicQuery
        {
            Filter = filter,
            Sort = sort
        };
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
