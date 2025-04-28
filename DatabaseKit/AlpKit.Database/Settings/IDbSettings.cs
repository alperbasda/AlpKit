namespace AlpKit.Database.Settings;

public interface IDbSettings
{
    string DatabaseName { get; set; }
    string ConnectionString { get; set; }
}
