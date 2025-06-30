using AlpKit.Common.Constants;
using AlpKit.Presentation.Api.Constants;
using AlpKit.Presentation.Api.Models;
using AlpKit.Presentation.Api.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpKit.Presentation.Api.Filters;

public class FillTokenParameterFilter : IEndpointFilter
{
    private static List<string> _clientDataKeys;
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var tokenParameters = context.HttpContext.RequestServices.GetService<TokenParameters>()!;

        tokenParameters.IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? " ";
        tokenParameters.UserLanguage = context.HttpContext.Request.Headers[HeaderConstants.Language].ToString() ?? AppConstants.DefaultLanguage;

        CultureInfo.CurrentCulture = new CultureInfo(tokenParameters.UserLanguage);

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderConstants.Authorization, out StringValues jwt))
            return await next(context);

        

        if (jwt.ToString().Split(' ').Length > 1)
            jwt = jwt.ToString().Split(' ')[1];


        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        if (token == null)
            return await next(context);

        tokenParameters.AccessToken = jwt!;

        setClientData(context, tokenParameters);

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

    private void setClientData(EndpointFilterInvocationContext context, TokenParameters tokenParameters)
    {
        if (_clientDataKeys == null)
        {
            _clientDataKeys = context.HttpContext.RequestServices.GetService<AuthSettings>()!.ClientDataKeys;
        }

        foreach (var item in _clientDataKeys)
        {
            var headerData = context.HttpContext.Request.Headers[item];
            if (string.IsNullOrEmpty(headerData))
                continue;
            tokenParameters.Data[item] = headerData.ToString();
        }
    }
}
