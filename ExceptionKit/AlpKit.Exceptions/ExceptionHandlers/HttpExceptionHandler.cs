using AlpKit.Common.Constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AlpKit.Exceptions.ExceptionHandlers;

public class HttpExceptionHandler : IExceptionHandler
{
    private HttpContext? _context;
    public HttpContext Context
    {
        get => _context ?? throw new ArgumentNullException(nameof(_context));
        set => _context = value;
    }

    public bool IsAjaxRequest { get; set; } = true;

    public Task HandleExceptionAsync(Exception exception)
    {
        Context.Response.ContentType = "application/json";
        Context.Response.StatusCode = exception.Data[ExceptionDataConstans.StatusCodeDataName] as int? ?? 500;
        
        return Context.Response.WriteAsync(JsonConvert.SerializeObject(new { Errors = exception.Message }));
    }
}
