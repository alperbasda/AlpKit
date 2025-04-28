using Microsoft.AspNetCore.Http;

namespace AlpKit.Exceptions.ExceptionHandlers;

public interface IExceptionHandler
{
    public HttpContext Context { get; set; }
    public bool IsAjaxRequest { get; set; }
    Task HandleExceptionAsync(Exception exception);
}
