using AlpKit.MultiLanguage.Localizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace AlpKit.MultiLanguage.RouteLocalizer;

public class LocalizedTransformer : DynamicRouteValueTransformer
{
    List<JsonLocalization> _translations = new List<JsonLocalization>();
    public LocalizedTransformer()
    {
        _translations = JsonConvert.DeserializeObject<List<JsonLocalization>>(File.ReadAllText(@"route_localization.json"))!;
    }

    public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        if (values.ContainsKey("controller"))
        {
            var controllerName = values["controller"]?.ToString();
            values["controller"] = _translations.FirstOrDefault(x => x.LocalizedValue.Values.Contains(controllerName?.ToLower()))?.Key ?? controllerName;
        }

        if (values.ContainsKey("action"))
        {
            var actionName = values["action"]?.ToString();
            values["action"] = _translations.FirstOrDefault(x => x.LocalizedValue.Values.Contains(actionName?.ToLower()))?.Key ?? actionName;

        }

        return new ValueTask<RouteValueDictionary>(values);
    }
}
