using AlpKit.Database.Settings;
using System.ComponentModel.DataAnnotations;

namespace AlpKit.MongoAdapter.Settings;

public class MongoSettings : IDbSettings
{
    [Required]
    public string DatabaseName { get; set; } = default!;
    [Required]
    public string ConnectionString { get; set; } = default!;
}
