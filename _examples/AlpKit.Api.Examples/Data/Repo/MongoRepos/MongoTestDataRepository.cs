using AlpKit.Api.Examples.Data.Contexts;
using AlpKit.Api.Examples.Models;
using AlpKit.Database.Repositories;

namespace AlpKit.Api.Examples.Data.Repo.MongoRepos;

public class MongoTestDataRepository(MongoContext context) : RepositoryBase<MongoData, MongoContext>(context), IMongoTestDataRepository
{
}
