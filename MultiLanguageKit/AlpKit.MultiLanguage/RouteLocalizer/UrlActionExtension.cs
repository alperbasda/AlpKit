using AlpKit.MultiLanguage.Localizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace AlpKit.MultiLanguage.RouteLocalizer;

public static class UrlActionExtension
{
    private static List<JsonLocalization> _translations = new();

    static UrlActionExtension()
    {
        _translations = JsonConvert.DeserializeObject<List<JsonLocalization>>(File.ReadAllText(@"route_localization.json"))!;
    }

    public static string LanguageAction(this IUrlHelper helper, string action, string controller, object? values = null)
    {
        var culture = CultureInfo.CurrentCulture.Name;

        List<string> routeParams = [];

        if (values != null)
        {
            var properties = values.GetType().GetProperties();
            
            foreach (var prop in properties)
            {
                var value = prop.GetValue(values)?.ToString();
                if (string.IsNullOrEmpty(value)) continue;

                routeParams.Add(value);
            }
        }


        string translatedController = _translations
            .FirstOrDefault(x => x.Key == controller.ToLower())?
            .LocalizedValue.GetValueOrDefault(culture.ToString(), controller) ?? controller;

        string translatedAction = _translations
            .FirstOrDefault(x => x.Key == action.ToLower())?
            .LocalizedValue.GetValueOrDefault(culture.ToString(), action) ?? action;
        if (translatedAction == "Index")
        {
            translatedAction = "";
        }

        string url = $"/{culture}/{translatedController}/{translatedAction}/{string.Join("/", routeParams)}";
        if (url.EndsWith("/"))
        {
            url = url.Substring(0, url.Length - 1);
        }
        return url;
    }
}
