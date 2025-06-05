using AlpKit.Database.Settings;
using System.ComponentModel.DataAnnotations;

namespace AlpKit.MsSqlAdapter.Settings;

public class MsSqlSettings : IDbSettings
{
    [Obsolete]
    public string DatabaseName { get; set; } = "Unknown";
    [Required]
    public string ConnectionString { get; set; } = default!;
    [Required]
    public bool ApplyMigration { get; set; } = default!;
}
