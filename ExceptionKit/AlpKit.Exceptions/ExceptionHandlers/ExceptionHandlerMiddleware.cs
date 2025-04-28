using AlpKit.Common.Constants;
using AlpKit.Exceptions.HttpProblemDetails;
using AlpKit.Logging.Loggers;
using Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AlpKit.Exceptions.ExceptionHandlers;

public static class ExceptionHandlerMiddlewareExtension
{
    public static IServiceCollection AddExceptionHandler(this IServiceCollection collection, bool isWebProject)
    {
        if (isWebProject)
        {
            collection.AddSingleton<IExceptionHandler, RedirectExceptionHandler>();
        }
        else
        {
            collection.AddSingleton<IExceptionHandler, HttpExceptionHandler>();
        }

        return collection;
    }

    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LoggerServiceBase _loggerService;
    private readonly IExceptionHandler _exceptionHandler;

    public ExceptionHandlerMiddleware(RequestDelegate next, LoggerServiceBase loggerService, IExceptionHandler exceptionHandler)
    {
        _next = next;
        _loggerService = loggerService;
        _exceptionHandler = exceptionHandler;
    }

    public async Task Invoke(HttpContext context, ITempDataProvider tempDataProvider)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await LogException(context, exception);
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task LogException(HttpContext context, Exception exception)
    {

        List<LogParameter> logParameters =
        [
            new LogParameter{Type=context.GetType().Name, Value=exception.ToString()}
        ];



        LogDetailWithException logDetail = new()
        {
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "",
            ExceptionMessage = exception.Message,
            MethodName = _next.Method.Name,
            Parameters = logParameters,
            User = context.User.Identity?.Name ?? "?"
        };

        var logType = exception.Data[ExceptionDataConstans.LogTypeDataName] as LogType? ?? LogType.Error;
        switch (logType)
        {
            case LogType.Info:
                _loggerService.Info(JsonSerializer.Serialize(logDetail));
                break;
            case LogType.Warn:
                _loggerService.Warn(JsonSerializer.Serialize(logDetail));
                break;
            case LogType.Error:
                _loggerService.Error(JsonSerializer.Serialize(logDetail));
                break;
        }

        return Task.CompletedTask;
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _exceptionHandler.Context = context;
        _exceptionHandler.IsAjaxRequest = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        return _exceptionHandler.HandleExceptionAsync(exception);
    }
}
