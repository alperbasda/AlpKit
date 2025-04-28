using Serilog;

namespace AlpKit.Logging.Loggers.FileLoggers;

public class FileLogger : LoggerServiceBase
{
    public FileLogger(FileLoggerConfiguration fileLoggerConfiguration)
    {
        string logFilePath = string.Format(format: "{0}{1}", arg0: Directory.GetCurrentDirectory() + fileLoggerConfiguration.FolderPath, arg1: ".txt");

        Logger = new LoggerConfiguration().WriteTo.File(
            logFilePath, rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: null,
            fileSizeLimitBytes: 5000000,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
            ).CreateLogger();
    }
}
