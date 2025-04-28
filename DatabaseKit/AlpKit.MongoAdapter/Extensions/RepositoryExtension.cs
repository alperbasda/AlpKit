using AlpKit.MongoAdapter.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AlpKit.MongoAdapter.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection collection, MongoSettings settings)
        where TContext : DbContext, new()
    {

        collection.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            return new MongoClient(settings.ConnectionString);
        });

        collection.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(settings.DatabaseName);
            var optionsBuilder = new DbContextOptionsBuilder<TContext>().UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
            return (TContext)ActivatorUtilities.CreateInstance(sp, typeof(TContext), optionsBuilder.Options);
        });
        return collection;
    }
}
