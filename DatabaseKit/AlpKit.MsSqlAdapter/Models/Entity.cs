using AlpKit.Database.Models;

namespace AlpKit.MsSqlAdapter.Models;

public class Entity : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public DateTime? DeletedTime { get; set; }
}
