using System.Net;
using System.Text.Json.Serialization;

namespace IdentityService.DTOs;

public class ApiResponse<T>
{
    public bool IsSuccessfull { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<string> Errors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string> ErrorsByField { get; set; }

    private ApiResponse() { }

    public static ApiResponse<T> Success(
        T data,
        string message = null,
        int statusCode = (int)HttpStatusCode.OK
    ) =>
        new ApiResponse<T>
        {
            IsSuccessfull = true,
            Data = data,
            Message = message,
            StatusCode = statusCode,
            Errors = null,
        };

    public static ApiResponse<T> Success(
        string message = null,
        int statusCode = (int)HttpStatusCode.OK
    ) =>
        new ApiResponse<T>
        {
            IsSuccessfull = true,
            Message = message,
            StatusCode = statusCode,
            Errors = null,
        };

    public static ApiResponse<T> Failed(
        string message,
        IEnumerable<string> errors = null,
        int statusCode = (int)HttpStatusCode.BadRequest
    ) =>
        new ApiResponse<T>
        {
            IsSuccessfull = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode,
        };

    public static ApiResponse<T> Failed(string message, IDictionary<string, string> errors) =>
        new ApiResponse<T>
        {
            IsSuccessfull = false,
            Message = message,
            ErrorsByField = errors,
            StatusCode = (int)HttpStatusCode.BadRequest,
        };

    public static ApiResponse<T> NotFound(
        string message,
        int statusCode = (int)HttpStatusCode.NotFound
    ) =>
        new ApiResponse<T>
        {
            IsSuccessfull = false,
            Message = message,
            StatusCode = statusCode,
        };

}
