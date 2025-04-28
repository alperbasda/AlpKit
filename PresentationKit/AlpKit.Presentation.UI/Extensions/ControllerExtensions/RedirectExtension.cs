using Microsoft.AspNetCore.Mvc;

namespace AlpKit.Presentation.UI.Extensions.ControllerExtensions
{
    public static class RedirectExtension
    {
        public static CustomRedirectActionResult Success(this RedirectToActionResult instance, string message)
        {
            return new CustomRedirectActionResult(instance.ActionName, instance.ControllerName, instance.RouteValues, message, "Success");
        }
        public static CustomRedirectActionResult Error(this RedirectToActionResult instance, string message)
        {
            return new CustomRedirectActionResult(instance.ActionName, instance.ControllerName, instance.RouteValues, message, "Error");
        }
        public static CustomActionResult Success(this RedirectResult instance, string message)
        {
            return new CustomActionResult(instance, message, "Success");
        }
        public static CustomActionResult Error(this RedirectResult instance, string message)
        {
            return new CustomActionResult(instance, message, "Error");
        }

        public static CustomViewResult Success(this ViewResult instance, string message)
        {
            return new CustomViewResult(instance, message, "Success");
        }
        public static CustomViewResult Error(this ViewResult instance, string message)
        {
            return new CustomViewResult(instance, message, "Error");
        }
    }
}