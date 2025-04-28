namespace AlpKit.Database.Repositories;

public interface IQuery<T>
{
    IQueryable<T> Query();
}
