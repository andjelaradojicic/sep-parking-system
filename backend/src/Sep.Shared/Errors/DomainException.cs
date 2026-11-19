namespace Sep.Shared.Errors;

public class DomainException(
    int statusCode,
    string code,
    string title,
    IDictionary<string, string[]>? errors = null) : Exception(title)
{
    public int StatusCode { get; } = statusCode;
    public string Code { get; } = code;
    public IDictionary<string, string[]>? Errors { get; } = errors;
}
