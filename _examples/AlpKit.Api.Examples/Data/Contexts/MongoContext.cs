using AlpKit.Api.Examples.Models;
using Microsoft.EntityFrameworkCore;

namespace AlpKit.Api.Examples.Data.Contexts;

public class MongoContext : DbContext
{
    public MongoContext(DbContextOptions<MongoContext> opt) : base(opt)
    {

    }

    [Obsolete("Sadece DbContexti eklerken hata vermemesi için kullanılıyor.")]
    public MongoContext()
    {

    }

    public DbSet<MongoData> MongoDatas { get; set; } = default!;

}

