using AlpKit.Common.Models.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace AlpKit.Presentation.Api.Filters;

public class FluentValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next.Invoke(context);
        }

        var arg = context.Arguments.OfType<T>().FirstOrDefault();
        if (arg is null)
        {
            return await next.Invoke(context);
        }

        var validationResult = await validator.ValidateAsync(arg);

        if (!validationResult.IsValid)
        {
            return Response<T>.Fail(string.Join("\n", validationResult.Errors.Select(w => w.ErrorMessage)), HttpStatusCode.BadRequest);
        }

        return await next.Invoke(context);

    }

}
