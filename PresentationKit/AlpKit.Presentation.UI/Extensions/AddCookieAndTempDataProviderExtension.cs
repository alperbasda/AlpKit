using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace AlpKit.Presentation.UI.Extensions;

public static class AddCookieAndTempDataProviderExtension
{
    public static IServiceCollection AddCookieAndTempData(this IServiceCollection collection)
    {
        collection.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        collection.ConfigureNonBreakingSameSiteCookies();

        collection.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(20);
            options.Cookie.IsEssential = true;
        });


        collection.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.IsEssential = true;
        });

        collection.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

        return collection;
    }
}
