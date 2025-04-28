namespace AlpKit.Logging.Loggers.ElasticLoggers;

public class ElasticLoggerConfiguration
{
    public string AppName { get; set; } = default!;

    public string Uri { get; set; } = default!;

    public string DataSetName { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;
}
