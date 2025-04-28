using AlpKit.Database.DynamicFilter.Models;
using Microsoft.EntityFrameworkCore;

namespace AlpKit.Database.DynamicFilter;


public static class IQueryablePaginateExtensions
{
    public static async Task<Paginate<T>> ToPaginateAsync<T>(
        this IQueryable<T> source,
        int index,
        int size,
        CancellationToken cancellationToken = default
        )
    {
        if (index <= 0)
        {
            index = 1;
        }
        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

        List<T> items = await source.Skip((index - 1) * size).Take(size).TagWith(nameof(T)).ToListAsync(cancellationToken).ConfigureAwait(false);

        Paginate<T> list = new()
        {
            Index = index,
            Count = count,
            Items = items,
            Size = size,
            Pages = (int)Math.Ceiling(count / (double)size)
        };
        return list;

    }

    public static Paginate<T> ToPaginate<T>(this IQueryable<T> source, int index, int size)
    {
        if (index <= 0)
        {
            index = 1;
        }

        int count = source.Count();
        var items = source.Skip((index - 1) * size).Take(size).TagWith(nameof(T)).ToList();

        Paginate<T> list =
            new()
            {
                Index = index,
                Size = size,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)size)
            };
        return list;
    }
}

