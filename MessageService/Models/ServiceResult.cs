using System;

namespace MessageService.Models;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }

    public int StatusCode { get; set; } 
    public IEnumerable<string> Errors { get; set; }

    private ServiceResult(bool isSuccess, T data, string message, int statusCode, IEnumerable<string> errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        StatusCode = statusCode;
        Errors = errors ?? new List<string>();
    }

    public static ServiceResult<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ServiceResult<T>(true, data, message, 200);
    }

    public static ServiceResult<T> Created(T data, string message = "Resource created successfully")
    {
        return new ServiceResult<T>(true, data, message, 201);
    }

    public static ServiceResult<T> NotFound(string message = "Resource not found")
    {
        return new ServiceResult<T>(false, default, message, 404);
    }

    public static ServiceResult<T> BadRequest(string message, IEnumerable<string> errors = null)
    {
        return new ServiceResult<T>(false, default, message, 400, errors);
    }

    public static ServiceResult<T> Unauthorized(string message = "Unauthorized access")
    {
        return new ServiceResult<T>(false, default, message, 401);
    }

    public static ServiceResult<T> Forbidden(string message = "Access forbidden")
    {
        return new ServiceResult<T>(false, default, message, 403);
    }

    public static ServiceResult<T> Error(string message = "An error occurred", IEnumerable<string> errors = null)
    {
        return new ServiceResult<T>(false, default, message, 500, errors);
    }
}
