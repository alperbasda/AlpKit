﻿using Microsoft.AspNetCore.Http;

namespace AlpKit.Presentation.UI.Extensions.ControllerExtensions
{
    public static class AjaxRequestExtension
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}