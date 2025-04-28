using Microsoft.Extensions.DependencyInjection;

namespace AlpKit.Logging.Loggers.ElasticLoggers;

public static class ElasticLoggerMiddleware
{
    public static void AddElasticLogger(this IServiceCollection collection)
    {
        collection.AddSingleton<LoggerServiceBase, ElasticLogger>();
    }
}
