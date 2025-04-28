using Microsoft.Extensions.DependencyInjection;

namespace AlpKit.Logging.Loggers.MsSqlLoggers;

public static class MsSqlLoggerMiddleware
{
    public static void AddMsSqlLogger(this IServiceCollection collection)
    {
        collection.AddSingleton<LoggerServiceBase, MsSqlLogger>();
    }
}
