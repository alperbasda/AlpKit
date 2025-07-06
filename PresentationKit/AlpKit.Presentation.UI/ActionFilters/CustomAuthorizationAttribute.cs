using AlpKit.Common.Constants;
using AlpKit.Presentation.UI.Constants;
using AlpKit.Presentation.UI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace AlpKit.Presentation.UI.ActionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    private static TokenValidationParameters? _tokenValidationParameter = null;
    private readonly HashSet<string> _scopes;

    public CustomAuthorizationAttribute(params string[] scopes)
    {
        _scopes = [.. scopes];
    }

    public CustomAuthorizationAttribute()
    {
        _scopes = [];
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var endpoint = context.ActionDescriptor.EndpointMetadata;
        if (endpoint.OfType<AllowAnonymousAttribute>().Any())
            return;

        var httpContext = context.HttpContext;
        AuthSettings authSettings = context.HttpContext.RequestServices.GetService<AuthSettings>()!;



        if (!httpContext.Request.Cookies.TryGetValue(WebConstants.Authorization, out string? jwt))
            throwAuthorizationException(authSettings);

        jwt = ExtractBearerToken(jwt);

        ValidateToken(context.HttpContext, jwt!, authSettings);



        if (_scopes.Any())
        {
            var tokenScopes = httpContext.User.Claims
                .Where(c => c.Type == "scope")
                .Select(c => c.Value);

            if (!_scopes.Any(scope => tokenScopes.Contains(scope)))
                throwAuthorizationException(authSettings);
        }

    }

    private static string? ExtractBearerToken(string? jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt)) return jwt;
        if (jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return jwt.Substring(7);
        return jwt;
    }

    private void throwAuthorizationException(AuthSettings authSettings)
    {
        var ex = new Exception("Token is Invalid.");
        ex.Data[ExceptionDataConstans.RedirectUrlDataName] = authSettings.RefreshTokenEndpoint;
        ex.Data[ExceptionDataConstans.LogTypeDataName] = LogType.Warn;
        ex.Data[ExceptionDataConstans.TypeDataName] = ExceptionDataConstans.InvalidAccessToken;
        ex.Data[ExceptionDataConstans.StatusCodeDataName] = HttpStatusCode.Unauthorized;

        throw ex;
    }

    private void ValidateToken(HttpContext context, string token, AuthSettings authSettings)
    {
        try
        {
            if (_tokenValidationParameter == null)
            {
                _tokenValidationParameter = new TokenValidationParameters()
                {
                    ValidIssuer = authSettings!.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecurityKey)),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            }
            var handler = new JwtSecurityTokenHandler();
            context.User = handler.ValidateToken(token, _tokenValidationParameter, out _);
        }
        catch (Exception e)
        {
            throwAuthorizationException(authSettings);
        }
    }

}
