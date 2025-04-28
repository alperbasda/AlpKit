
using AlpKit.Api.Examples.Data.Contexts;
using AlpKit.Api.Examples.Data.Repo.MongoRepos;
using AlpKit.Api.Examples.Data.Repo.MsSqlRepos;
using AlpKit.MongoAdapter.Extensions;
using AlpKit.MongoAdapter.Settings;
using AlpKit.MsSqlAdapter.Extensions;
using AlpKit.MsSqlAdapter.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddMsSqlDbContext<MssqlContext>( new MsSqlSettings { ConnectionString = "your_connection_string",DatabaseName = "not_required" });
builder.Services.AddScoped<IMsSqlTestDataRepository, MsSqlTestDataRepository>();

builder.Services.AddMongoDbContext<MongoContext>(new MongoSettings { ConnectionString = "your_connection_string", DatabaseName = "mongo_test"});
builder.Services.AddScoped<IMongoTestDataRepository, MongoTestDataRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
