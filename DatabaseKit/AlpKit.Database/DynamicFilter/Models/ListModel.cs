namespace AlpKit.Database.DynamicFilter.Models;

public abstract class BasePageableModel
{
    public int Size { get; set; }
    public int Index { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }

}

public class ListModel<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items
    {
        get => _items ??= new List<T>();
        set => _items = value;
    }

    public PageRequest PageRequest { get; set; }

    public DynamicQuery DynamicQuery { get; set; }
}
