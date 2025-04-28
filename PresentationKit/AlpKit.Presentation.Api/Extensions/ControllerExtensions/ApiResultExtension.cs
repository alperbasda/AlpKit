using AlpKit.Common.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace AlpKit.Presentation.Api.Extensions.ControllerExtensions;

public static class ApiResultExtension
{
    public static IResult ToResult<T>(this Response<T> response)
    {
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.Unauthorized:
                return TypedResults.Unauthorized();
            case System.Net.HttpStatusCode.BadRequest:
                return TypedResults.BadRequest(new
                {
                    errors = response.Errors
                });
            case System.Net.HttpStatusCode.InternalServerError:
                return TypedResults.BadRequest(new
                {
                    errors = response.Errors
                });
            default:
                return TypedResults.Ok(new
                {
                    data = response.Data,
                    errors = response.Errors
                });
        }
    }
}