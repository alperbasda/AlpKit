using AlpKit.Api.Examples.Models;
using AlpKit.Database.Repositories;

namespace AlpKit.Api.Examples.Data.Repo.MongoRepos;

public interface IMongoTestDataRepository : IAsyncRepository<MongoData>
{
}
