using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace AlpKit.Logging.Loggers.MsSqlLoggers;

public class MsSqlLogger : LoggerServiceBase
{
    public MsSqlLogger(MsSqlLoggerConfiguration msSqlLoggerConfiguration)
    {
        MSSqlServerSinkOptions sinkOptions = new()
        {
            TableName = msSqlLoggerConfiguration.TableName,
            AutoCreateSqlDatabase = msSqlLoggerConfiguration.AutoCreateSqlTable
        };

        ColumnOptions columnOptions = new();

        global::Serilog.Core.Logger seriLogConfig = new LoggerConfiguration().WriteTo
            .MSSqlServer(msSqlLoggerConfiguration.ConnectionString, sinkOptions, columnOptions: columnOptions)
            .CreateLogger();

        Logger = seriLogConfig;
    }
}
