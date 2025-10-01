namespace Retail.BuildingBlocks.Primitives;

public sealed record Error(string Code, string Message);

public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Ok() => new(true, null);
    public static Result Fail(string Code, string Message) => new(false, new Error(Code, Message));
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, Error? error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, null);
    public static new Result<T> Fail(string Code, string Message) => new(false, default, new Error(Code, Message));
}