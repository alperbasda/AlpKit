using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AlpKit.Presentation.UI.Extensions.ControllerExtensions
{
    public class CustomViewResult : ViewResult
    {
        public ActionResult BaseResult { get; }

        private readonly string _message;

        private readonly string _type;

        public CustomViewResult(ActionResult redirectBaseResult, string message, string type)
        {
            BaseResult = redirectBaseResult;
            _message = message;
            _type = type;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            ITempDataDictionaryFactory factory = context.HttpContext.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
            ITempDataDictionary tempData = factory.GetTempData(context.HttpContext);
            tempData[_type] = _message;
            BaseResult.ExecuteResult(context);

            await base.ExecuteResultAsync(context);
        }


    }
}