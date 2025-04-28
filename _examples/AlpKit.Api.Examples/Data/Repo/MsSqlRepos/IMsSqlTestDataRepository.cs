using AlpKit.Api.Examples.Models;
using AlpKit.Database.Repositories;

namespace AlpKit.Api.Examples.Data.Repo.MsSqlRepos;

public interface IMsSqlTestDataRepository : IAsyncRepository<MssqlData>
{
}
