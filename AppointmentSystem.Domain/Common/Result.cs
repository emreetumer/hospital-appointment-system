namespace AppointmentSystem.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public string Message { get; private set; }
    public List<string> Errors { get; private set; }

    private Result(bool isSuccess, T data, string message, List<string> errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data, string message = "Operation successful")
    {
        return new Result<T>(true, data, message, null);
    }

    public static Result<T> Failure(string message, List<string> errors = null)
    {
        return new Result<T>(false, default, message, errors);
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default, "Operation failed", errors);
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public List<string> Errors { get; private set; }

    private Result(bool isSuccess, string message, List<string> errors)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result Success(string message = "Operation successful")
    {
        return new Result(true, message, null);
    }

    public static Result Failure(string message, List<string> errors = null)
    {
        return new Result(false, message, errors);
    }

    public static Result Failure(List<string> errors)
    {
        return new Result(false, "Operation failed", errors);
    }
}
