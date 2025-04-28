using AlpKit.Common.Models.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace AlpKit.Presentation.UI.ActionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class FluentValidationAttribute<T> : Attribute, IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator == null)
        {
            await next();
            return;
        }

        var entity = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
        if (entity == null)
        {
            await next();
            return;
        }

        var validationResult = await validator.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));

            context.Result = new ObjectResult(Response<T>.Fail(errorMessage, HttpStatusCode.BadRequest))
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
            return;
        }

        await next();
    }
}
