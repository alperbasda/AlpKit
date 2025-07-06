using AlpKit.Common.Constants;
using AlpKit.Presentation.UI.Constants;
using AlpKit.Presentation.UI.Models;
using AlpKit.Presentation.UI.Settings;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpKit.Presentation.UI.ActionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class FillTokenParameterAttribute : Attribute, IAsyncActionFilter
{
    private static List<string> _clientDataKeys;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var tokenParameters = httpContext.RequestServices.GetRequiredService<TokenParameters>();
        var authSettings = httpContext.RequestServices.GetRequiredService<AuthSettings>();

        tokenParameters.IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? " ";
        tokenParameters.UserLanguage = httpContext.Request.Cookies[WebConstants.Language] ?? AppConstants.DefaultLanguage;

        if (!httpContext.Request.Cookies.TryGetValue(WebConstants.Authorization, out string? jwtRaw))
        {
            await next();
            return;
        }

        string? jwt = ExtractBearerToken(jwtRaw);
        if (string.IsNullOrWhiteSpace(jwt))
        {
            await next();
            return;
        }

        var handler = new JwtSecurityTokenHandler();

        JwtSecurityToken? token;
        try
        {
            token = handler.ReadJwtToken(jwt);
        }
        catch
        {
            await next();
            return;
        }

        if (token == null)
        {
            await next();
            return;
        }


        tokenParameters.AccessToken = jwt;

        setClientData(context, tokenParameters);

        var identity = new ClaimsIdentity(token.Claims, "jwt");
        var principal = new ClaimsPrincipal(identity);
        httpContext.User = principal;

        tokenParameters.Scopes = token.Claims
                                .Where(c => c.Type == "scope")
                                .Select(c => c.Value)
                                .ToArray();

        tokenParameters.UserName = principal.Identity?.Name ?? string.Empty;

        tokenParameters.UserId = Guid.Empty;

        var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userId, out var guid))
            tokenParameters.UserId = guid;

        tokenParameters.IsSuperUser = token.Claims.Any(c => c.Type == "scope" && c.Value == WebConstants.AdminScope);

        await next();
    }


    private static string? ExtractBearerToken(string? jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt)) return jwt;
        if (jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return jwt.Substring(7);
        return jwt;
    }


    private void setClientData(ActionExecutingContext context, TokenParameters tokenParameters)
    {
        if (_clientDataKeys == null)
        {
            _clientDataKeys = context.HttpContext.RequestServices.GetService<AuthSettings>()!.ClientDataKeys;
        }

        foreach (var item in _clientDataKeys)
        {
            var headerData = context.HttpContext.Request.Cookies[item];
            if (string.IsNullOrEmpty(headerData))
                continue;
            tokenParameters.Data[item] = headerData.ToString();
        }
    }
}
