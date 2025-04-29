using AlpKit.Logging.Extensions;
using AlpKit.Logging.Loggers;
using AlpKit.Logging.Loggers.FileLoggers;
using AlpKit.Logging.Settings;
using AlpKitUseExceptionHandler = AlpKit.Exceptions.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

//File logger eklendi. 
var fileLoggerConfig = new FileLoggerConfiguration
{
    FolderPath = "/logs/",
};
builder.Services.AddSingleton(fileLoggerConfig);
builder.Services.AddFileLogger();

//Exception handling
AlpKitUseExceptionHandler.ExceptionHandlerMiddlewareExtension.AddExceptionHandler(builder.Services, false);


//RequestLogger i�in.
var requestLoggingSettings = new RequestLoggingSettings
{
    //Loglanmamas�n� istedi�iniz urlleri yaz�n (startwith ile �al���r.)
    ExcludedPaths = [],
};
builder.Services.AddSingleton(requestLoggingSettings);


builder.Services.AddControllersWithViews();



builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//E�er requestler loglanmak istenirse sat�r� eklemeliyiz.
var fileLogger = app.Services.GetRequiredService<LoggerServiceBase>();
app.UseRequestLogging(fileLogger);

//Exception handling
AlpKitUseExceptionHandler.ExceptionHandlerMiddlewareExtension.UseExceptionHandler(app);


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
