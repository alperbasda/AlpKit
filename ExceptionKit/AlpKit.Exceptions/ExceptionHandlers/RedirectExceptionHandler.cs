using AlpKit.Common.Constants;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace AlpKit.Exceptions.ExceptionHandlers;

public class RedirectExceptionHandler(ITempDataProvider _tempDataProvider) : IExceptionHandler
{
    private HttpContext? _context;
    public HttpContext Context
    {
        get => _context ?? throw new ArgumentNullException(nameof(_context));
        set => _context = value;
    }

    private bool _isAjaxRequest = false;
    public bool IsAjaxRequest
    {
        get => _isAjaxRequest;
        set => _isAjaxRequest = value;
    }
    //Burda ajax ise json dönecegiz. url varsa redirect yapacagiz.Boş ise aynı sayfayı basacagız.
    public async Task HandleExceptionAsync(Exception exception)
    {
        if (IsAjaxRequest)
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = exception.Data[ExceptionDataConstans.StatusCodeDataName] as int? ?? 500;

            await Context.Response.WriteAsync(JsonConvert.SerializeObject(new { Errors = new[] { exception.Message, exception.StackTrace ?? "" } }));
        }
        else
        {
            var dict = new Dictionary<string, object>
                {
                    { "Error", exception.Message }
                };
            _tempDataProvider.SaveTempData(Context, dict);

            var redirect = exception.Data[ExceptionDataConstans.RedirectUrlDataName] as string;
            if (string.IsNullOrEmpty(redirect))
            {
                redirect = Context.Request.Path;
            }
            Context.Response.Redirect(redirect);
        }


    }

}
