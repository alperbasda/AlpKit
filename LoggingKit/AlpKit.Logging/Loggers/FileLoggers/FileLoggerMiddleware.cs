using Microsoft.Extensions.DependencyInjection;

namespace AlpKit.Logging.Loggers.FileLoggers;

public static class FileLoggerMiddleware
{
    public static void AddFileLogger(this IServiceCollection collection)
    {
        collection.AddSingleton<LoggerServiceBase, FileLogger>();
    }
}
