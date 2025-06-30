using AlpKit.MsSqlAdapter.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlpKit.MsSqlAdapter.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddMsSqlDbContext<TContext>(this IServiceCollection collection, MsSqlSettings settings, bool userGeo = false)
        where TContext : DbContext, new()
    {
        collection.AddDbContext<TContext>(options => options.UseSqlServer(settings.ConnectionString, userGeo ? x => x.UseNetTopologySuite() : null));
        return collection;
    }
}
