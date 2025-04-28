using AlpKit.Common.Constants;
using AlpKit.Presentation.UI.Constants;
using AlpKit.Presentation.UI.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpKit.Presentation.UI.ActionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class FillTokenParameterAttribute : Attribute, IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var tokenParameters = context.HttpContext.RequestServices.GetService<TokenParameters>()!;

        tokenParameters.IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? " ";
        tokenParameters.UserLanguage = context.HttpContext.Request.Headers[WebConstants.Language].ToString() ?? AppConstants.DefaultLanguage;

        if (!context.HttpContext.Request.Cookies.TryGetValue(WebConstants.Authorization, out string jwt))
            await next();



        if (jwt.ToString().Split(' ').Length > 1)
            jwt = jwt.ToString().Split(' ')[1];


        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        if (token == null)
            await next();

        tokenParameters.AccessToken = jwt;

        var identity = new ClaimsIdentity(token!.Claims, "basic");
        context.HttpContext.User = new ClaimsPrincipal(identity);
        if (!string.IsNullOrEmpty(context.HttpContext?.User?.Identity?.Name))
            tokenParameters.UserName = context.HttpContext.User.Identity.Name;

        tokenParameters.UserId = Guid.Empty;
        var userIdClaim = token.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim))
            tokenParameters.UserId = Guid.Parse(userIdClaim);

        tokenParameters.IsSuperUser = token.Claims.Any(w => w.Type == "scope" && w.Value == WebConstants.AdminScope);


        context.HttpContext!.Response.HttpContext.User = new ClaimsPrincipal(identity);

        await next();
    }
}
