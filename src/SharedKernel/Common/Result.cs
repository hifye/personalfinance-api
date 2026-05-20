namespace SharedKernel.Common;

/// <summary>
/// Representa o resultado de uma operação, indicando sucesso ou falha.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Error { get; }

    public ErrorType ErrorType { get; }

    protected Result(bool isSuccess, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success() => new Result(true, null, ErrorType.None);

    public static Result Failure(string error, ErrorType type) => new Result(false, error, type);

    public Result Bind(Func<Result> func) => IsFailure ? this : func();

    public Result<T> Bind<T>(Func<Result<T>> func) =>
        IsFailure ? Result<T>.Failure(Error!, ErrorType) : func();

    public Result<T> Map<T>(Func<T> func) =>
        IsFailure ? Result<T>.Failure(Error!, ErrorType) : Result<T>.Success(func());

    public static Result Try(Action action, string errorMessage)
    {
        try
        {
            action();
            return Success();
        }
        catch
        {
            return Failure(errorMessage, ErrorType.Unexpected);
        }
    }

    public async Task<Result> BindAsync(Func<Task<Result>> func) => IsFailure ? this : await func();

    public async Task<Result<T>> BindAsync<T>(Func<Task<Result<T>>> func) =>
        IsFailure ? Result<T>.Failure(Error!, ErrorType) : await func();
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, string? error, ErrorType errorType, T? value)
        : base(isSuccess, error, errorType)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, null, ErrorType.None, value);

    public static new Result<T> Failure(string error, ErrorType errorType) =>
        new(false, error, errorType, default);

    public Result<K> Bind<K>(Func<T, Result<K>> func) =>
        IsFailure ? Result<K>.Failure(Error!, ErrorType) : func(Value!);

    public Result<K> Map<K>(Func<T, K> func) =>
        IsFailure ? Result<K>.Failure(Error!, ErrorType) : Result<K>.Success(func(Value!));

    public async Task<Result<K>> BindAsync<K>(Func<T, Task<Result<K>>> func) =>
        IsFailure ? Result<K>.Failure(Error!, ErrorType) : await func(Value!);

    public async Task<Result> BindAsync(Func<T, Task<Result>> func) =>
        IsFailure ? Result.Failure(Error!, ErrorType) : await func(Value!);

    public Result Bind(Func<T, Result> func) =>
        IsFailure ? Result.Failure(Error!, ErrorType) : func(Value!);
}
