using System.Net;
using System.Text.Json.Serialization;

namespace AlpKit.Common.Models.Responses;

public record NoContentResponse();
public interface IResponseBase
{
    HttpStatusCode StatusCode { get; set; }

    bool IsSuccessful { get; set; }

    List<string> Errors { get; set; }

}

/// <summary>
/// Api den dönen veriler Response ile dönülmesi zorunludur.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Response<T> : IResponseBase
{
    public T? Data { get; set; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }

    [JsonIgnore]
    public bool IsSuccessful { get; set; }

    public List<string> Errors { get; set; }

    // Static Factory Method
    public static Response<T> Success(T data, HttpStatusCode statusCode)
    {
        return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
    }

    public static Response<T> Success(HttpStatusCode statusCode)
    {
        return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful = true };
    }

    public static Response<T> Success(string message, HttpStatusCode statusCode)
    {
        return new Response<T> { Errors = new List<string>() { message }, StatusCode = statusCode, IsSuccessful = true };
    }

    public static Response<T> Fail(string error, HttpStatusCode statusCode)
    {
        return new Response<T> { Errors = new List<string>() { error }, StatusCode = statusCode, IsSuccessful = false };
    }

    public static Response<T> Fail(List<string> errors, HttpStatusCode statusCode)
    {
        return new Response<T> { Errors = errors, StatusCode = statusCode, IsSuccessful = false };
    }

    public static Response<T> Fail(T data, HttpStatusCode statusCode)
    {
        return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = false };
    }

    public static Response<T> Fail(T data, string error, HttpStatusCode statusCode)
    {
        return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = false, Errors = new List<string> { error } };
    }

    public static Response<T> Fail(T data, List<string> errors, HttpStatusCode statusCode)
    {
        return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = false, Errors = errors };
    }

    public Response<TNew> FailConvert<TNew>()
        where TNew : class
    {
        return new Response<TNew> { Errors = Errors, StatusCode = StatusCode, IsSuccessful = IsSuccessful, Data = null };
    }
}
