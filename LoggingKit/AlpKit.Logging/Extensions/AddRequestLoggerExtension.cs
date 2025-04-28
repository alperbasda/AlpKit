using AlpKit.Logging.Loggers;
using AlpKit.Logging.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace AlpKit.Logging.Extensions;

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, LoggerServiceBase logger)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>(logger);
    }
}

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LoggerServiceBase _logger;
    private readonly RequestLoggingSettings _options;

    public RequestLoggingMiddleware(RequestDelegate next, LoggerServiceBase logger, RequestLoggingSettings options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        var url = request.Path;

        if (_options.ExcludedPaths.Any(p => url.ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var method = request.Method;
        var queryString = request.QueryString.ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        string body = string.Empty;
        if (request.ContentLength > 0 && request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
            }
        }

        var logMessage = $"Request URL: {url},Time: {DateTime.Now} Method: {method}, QueryString: {queryString}, IP Address: {ipAddress}, Body: {body}";

        _logger.Info(logMessage);

        await _next(context);
    }
}

