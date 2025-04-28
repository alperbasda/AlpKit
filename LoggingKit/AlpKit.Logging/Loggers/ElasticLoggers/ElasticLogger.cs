using Elastic.Serilog.Sinks;
using Serilog.Debugging;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Transport;

namespace AlpKit.Logging.Loggers.ElasticLoggers;

public class ElasticLogger : LoggerServiceBase
{
    public ElasticLogger(ElasticLoggerConfiguration options)
    {
        SelfLog.Enable(Console.Error);



        base.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Filter.ByExcluding(Matching.FromSource("Microsoft"))
            .Filter.ByExcluding(Matching.FromSource("System"))
            .WriteTo.Console(theme: SystemConsoleTheme.Literate)
            .WriteTo.Elasticsearch([new Uri(options.Uri)], opts =>
            {
                opts.DataStream = new DataStreamName("logs", options.DataSetName, options.AppName);
                opts.BootstrapMethod = BootstrapMethod.Failure;
            }, transport =>
            {
                transport.Authentication(new BasicAuthentication(options.UserName, options.Password));
            })
            .Enrich.WithProperty("Application", options.AppName)
            .CreateLogger();

        Log.Logger = base.Logger;
    }
}