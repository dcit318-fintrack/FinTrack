namespace FinTrack.Shared.DTOs.Common;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public bool IsConflict { get; init; }

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(string errorMessage, Dictionary<string, string[]>? errors = null) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
        ValidationErrors = errors
    };

    public static Result<T> Conflict(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage,
        IsConflict = true
    };
}
