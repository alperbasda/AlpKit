
using AlpKit.Caching.Extensions;
using AlpKit.Caching.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddRedisCache(new CacheSettings { Host= "your_redis_host", Password= "your_redis_password", Port= "your_redis_port" });

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
