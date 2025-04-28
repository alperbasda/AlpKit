using AlpKit.Common.Constants;
using AlpKit.Presentation.Api.Constants;
using AlpKit.Presentation.Api.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AlpKit.Presentation.Api.Filters;

public class AuthorizationFilter : IEndpointFilter
{
    private static TokenValidationParameters? _tokenValidationParameter = null;
    private readonly HashSet<string> _scopes;

    public AuthorizationFilter(params string[] scopes)
    {
        _scopes = [.. scopes];
    }

    public AuthorizationFilter()
    {
        _scopes = new HashSet<string>();
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        AuthSettings authSettings = context.HttpContext.RequestServices.GetService<AuthSettings>()!;

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderConstants.Authorization, out StringValues jwt))
            throwAuthorizationException(authSettings);


        if (jwt.ToString().Split(' ').Length > 1)
            jwt = jwt.ToString().Split(' ')[1];

        ValidateToken(context.HttpContext, jwt!, authSettings);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        if (token == null || !_scopes.Any())
            throwAuthorizationException(authSettings);
        if (!token!.Claims.Where(w => w.Type == "scope").Select(w => w.Value).Any(_scopes.Contains))
            throwAuthorizationException(authSettings);

        return await next(context);
    }

    private void throwAuthorizationException(AuthSettings authSettings)
    {
        var ex = new Exception("Please sign in.");
        ex.Data[ExceptionDataConstans.RedirectUrlDataName] = authSettings.LoginUrl;
        ex.Data[ExceptionDataConstans.LogTypeDataName] = LogType.Warn;
        ex.Data[ExceptionDataConstans.TypeDataName] = ExceptionDataConstans.NotAuthorizedTypeDataName;
        throw ex;
    }

    private void ValidateToken(HttpContext context, string token, AuthSettings authSettings)
    {
        try
        {
            SetIfNullTokenValidationParametersOptions(context, authSettings);
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, _tokenValidationParameter, out _);
        }
        catch (Exception e)
        {
            throwAuthorizationException(authSettings);
        }
    }

    private void SetIfNullTokenValidationParametersOptions(HttpContext context, AuthSettings authSettings)
    {
        if (_tokenValidationParameter == null)
        {
            _tokenValidationParameter = new TokenValidationParameters()
            {
                ValidIssuer = authSettings!.Issuer,
                ValidAudience = authSettings.Audience,
                IssuerSigningKey = CreateSecurityKey(authSettings.SecurityKey),
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

    }

    private static SecurityKey CreateSecurityKey(string securityKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
    }
}
