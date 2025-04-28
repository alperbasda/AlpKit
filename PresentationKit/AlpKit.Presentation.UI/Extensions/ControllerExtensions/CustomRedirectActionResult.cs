using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AlpKit.Presentation.UI.Extensions.ControllerExtensions
{
    public class CustomRedirectActionResult : RedirectToActionResult
    {

        private readonly string _message;

        private readonly string _type;

        public override Task ExecuteResultAsync(ActionContext context)
        {
            ITempDataDictionaryFactory factory = context.HttpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
            ITempDataDictionary tempData = factory.GetTempData(context.HttpContext);
            tempData[_type] = _message;

            return base.ExecuteResultAsync(context);
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues, string message, string type) : base(actionName, controllerName, routeValues)
        {
            _message = message;
            _type = type;
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues) : base(actionName, controllerName, routeValues)
        {
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues, bool permanent) : base(actionName, controllerName, routeValues, permanent)
        {
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues, bool permanent, bool preserveMethod) : base(actionName, controllerName, routeValues, permanent, preserveMethod)
        {
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues, bool permanent, bool preserveMethod, string fragment) : base(actionName, controllerName, routeValues, permanent, preserveMethod, fragment)
        {
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues, bool permanent, string fragment) : base(actionName, controllerName, routeValues, permanent, fragment)
        {
        }

        public CustomRedirectActionResult(string actionName, string controllerName, object routeValues, string fragment) : base(actionName, controllerName, routeValues, fragment)
        {
        }
    }
}