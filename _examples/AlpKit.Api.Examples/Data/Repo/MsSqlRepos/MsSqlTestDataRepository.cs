using AlpKit.Api.Examples.Data.Contexts;
using AlpKit.Api.Examples.Models;
using AlpKit.Database.Repositories;

namespace AlpKit.Api.Examples.Data.Repo.MsSqlRepos;

public class MsSqlTestDataRepository(MssqlContext context) : RepositoryBase<MssqlData, MssqlContext>(context), IMsSqlTestDataRepository
{
}
