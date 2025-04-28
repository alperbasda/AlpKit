namespace AlpKit.Logging.Loggers.FileLoggers;

public class FileLoggerConfiguration
{
    public string FolderPath { get; set; }

    public FileLoggerConfiguration()
    {
        FolderPath = string.Empty;
    }

    public FileLoggerConfiguration(string folderPath)
    {
        FolderPath = folderPath;
    }
}
