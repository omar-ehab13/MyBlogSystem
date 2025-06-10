namespace BlogSystem.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new();
    public int StatusCode { get; private set; }

    protected Result() { }

    public static Result<T> Success(T data, string message = "Success", int statusCode = 200)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static Result<T> Failure(List<string> errors, int statusCode = 400, string message = "Operation failed")
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
    }
}
