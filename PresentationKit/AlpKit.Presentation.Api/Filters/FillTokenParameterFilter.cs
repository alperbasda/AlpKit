using AlpKit.Common.Constants;
using AlpKit.Presentation.Api.Constants;
using AlpKit.Presentation.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpKit.Presentation.Api.Filters;

public class FillTokenParameterFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var tokenParameters = context.HttpContext.RequestServices.GetService<TokenParameters>()!;

        tokenParameters.IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? " ";
        tokenParameters.UserLanguage = context.HttpContext.Request.Headers[HeaderConstants.Language].ToString() ?? AppConstants.DefaultLanguage;
        tokenParameters.DeviceId = context.HttpContext.Request.Headers[HeaderConstants.DeviceId].ToString() ?? AppConstants.Unknown;
        tokenParameters.DeviceToken = context.HttpContext.Request.Headers[HeaderConstants.DeviceToken].ToString() ?? AppConstants.Unknown;
        tokenParameters.Currency = context.HttpContext.Request.Headers[HeaderConstants.Currency].ToString() ?? "TRY";

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderConstants.Authorization, out StringValues jwt))
            return await next(context);

        

        if (jwt.ToString().Split(' ').Length > 1)
            jwt = jwt.ToString().Split(' ')[1];


        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        if (token == null)
            return await next(context);

        tokenParameters.AccessToken = jwt!;

        var identity = new ClaimsIdentity(token!.Claims, "basic");
        context.HttpContext.User = new ClaimsPrincipal(identity);
        if (!string.IsNullOrEmpty(context.HttpContext?.User?.Identity?.Name))
            tokenParameters.UserName = context.HttpContext.User.Identity.Name;

        tokenParameters.UserId = Guid.Empty;
        var userIdClaim = token.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim))
            tokenParameters.UserId = Guid.Parse(userIdClaim);

        tokenParameters.IsSuperUser = token.Claims.Any(w => w.Type == "scope" && w.Value == HeaderConstants.AdminScope);

        
        context.HttpContext!.Response.HttpContext.User = new ClaimsPrincipal(identity);

        return await next(context);
    }
}
