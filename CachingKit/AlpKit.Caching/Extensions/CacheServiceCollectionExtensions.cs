using AlpKit.Caching.Helpers;
using AlpKit.Caching.Settings;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AlpKit.Caching.Extensions;

public static class CacheServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, CacheSettings cacheSettings)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions
            {
                Password = cacheSettings.Password,
                EndPoints =
                    {
                        { cacheSettings.Host, int.Parse(cacheSettings.Port) }
                    },
            };
        });

        services.AddSingleton<ICacheHelper, CacheHelper>();

        return services;
    }

    public static IServiceCollection AddMemCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSingleton<ICacheHelper, CacheHelper>();
        return services;
    }
}
