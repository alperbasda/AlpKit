using AlpKit.MultiLanguage.Localizer;
using AlpKit.MultiLanguage.RouteLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace AlpKit.MultiLanguage.Extensions;

public static class AddMultiLanguageExtension
{
    public static Dictionary<string, CultureInfo> ApplicationCultures = [];
    public static IMvcBuilder AddControllersAndMultiLanguage(this IServiceCollection collection, CultureInfo[] cultures)
    {
        ApplicationCultures = cultures.ToDictionary(w => w.TwoLetterISOLanguageName, w => w);
        var mvcBuilder = collection.AddControllersWithViews()
                  .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
                  .AddDataAnnotationsLocalization();

        collection.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new("tr-TR");
            options.SupportedCultures = [.. ApplicationCultures.Values];
            options.SupportedUICultures = [.. ApplicationCultures.Values];
        });

        collection.AddSingleton<LocalizedTransformer>();
        collection.AddScoped<RequestLocalizationCookiesMiddleware>();
        collection.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        collection.AddSingleton<IStringLocalizer, JsonStringLocalizer>();

        return mvcBuilder;
    }
}

public class RequestLocalizationCookiesMiddleware : IMiddleware
{
    readonly CookieRequestCultureProvider _provider;

    public RequestLocalizationCookiesMiddleware(IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        => _provider = requestLocalizationOptions.Value.RequestCultureProviders.Where(x => x is CookieRequestCultureProvider).Cast<CookieRequestCultureProvider>().FirstOrDefault()!;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var pathSegments = context.Request.Path.Value?.ToLower().Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];

        var matchedCulture = pathSegments.FirstOrDefault(AddMultiLanguageExtension.ApplicationCultures.Keys.Contains) ?? "tr";

        CultureInfo.CurrentCulture = AddMultiLanguageExtension.ApplicationCultures[matchedCulture];

        if (_provider != null)
        {
            IRequestCultureFeature feature = context.Features.Get<IRequestCultureFeature>()!;
            if (feature != null)
                context.Response.Cookies.Append(_provider.CookieName, CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture));
        }

        await next(context);
    }
}

public static class RequestLocalizationCookiesMiddlewareExtensions
{
    public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, CultureInfo[] cultures)
    {
        var localizationOptions = new RequestLocalizationOptions
        {
            SupportedCultures = cultures,
            SupportedUICultures = cultures,
            DefaultRequestCulture = new RequestCulture("tr-TR"),
        };
        var requestProvider = new RouteDataRequestCultureProvider();
        localizationOptions.RequestCultureProviders.Insert(0, requestProvider);

        app.UseRequestLocalization(localizationOptions);

        return app.UseMiddleware<RequestLocalizationCookiesMiddleware>();

    }
}