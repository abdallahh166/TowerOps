namespace TelecomPM.Api.Errors;

public static class ApiErrorCodes
{
    public const string InternalError = "internal.error";
    public const string RequestFailed = "request.failed";
    public const string ValidationFailed = "request.validation_failed";
    public const string ResourceNotFound = "resource.not_found";
    public const string Unauthorized = "auth.unauthorized";
    public const string Forbidden = "auth.forbidden";
    public const string Conflict = "request.conflict";
    public const string BusinessRuleViolation = "business.rule_violation";
}
