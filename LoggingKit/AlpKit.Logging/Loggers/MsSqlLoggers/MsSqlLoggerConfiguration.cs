namespace AlpKit.Logging.Loggers.MsSqlLoggers;

public class MsSqlLoggerConfiguration
{
    public string ConnectionString { get; set; }
    public string TableName { get; set; }
    public bool AutoCreateSqlTable { get; set; }

    public MsSqlLoggerConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
    }

    public MsSqlLoggerConfiguration(string connectionString, string tableName, bool autoCreateTable)
    {
        ConnectionString = connectionString;
        TableName = tableName;
        AutoCreateSqlTable = autoCreateTable;

    }
}
