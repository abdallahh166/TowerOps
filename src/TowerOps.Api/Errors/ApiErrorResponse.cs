namespace TowerOps.Api.Errors;

public sealed class ApiErrorResponse
{
    public string Code { get; init; } = ApiErrorCodes.RequestFailed;
    public string Message { get; init; } = string.Empty;
    public string CorrelationId { get; init; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; init; }
    public IDictionary<string, string>? Meta { get; init; }
}
