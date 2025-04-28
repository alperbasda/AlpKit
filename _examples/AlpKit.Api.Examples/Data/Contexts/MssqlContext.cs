using AlpKit.Api.Examples.Models;
using Microsoft.EntityFrameworkCore;

namespace AlpKit.Api.Examples.Data.Contexts;

public class MssqlContext : DbContext
{
    public MssqlContext(DbContextOptions<MssqlContext> opt) : base(opt)
    {

    }

    [Obsolete("Sadece DbContexti eklerken hata vermemesi için kullanılıyor.")]
    public MssqlContext()
    {

    }

    public DbSet<MssqlData> MsSqlDatas { get; set; } = default!;

}
