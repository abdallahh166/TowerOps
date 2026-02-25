namespace TowerOps.Domain.Exceptions;

// ==================== Base Domain Exception ====================
public class DomainException : Exception
{
    public string? MessageKey { get; }
    public object[] MessageArguments { get; }

    public DomainException(string message) : base(message)
    {
        MessageArguments = Array.Empty<object>();
    }

    public DomainException(string message, string messageKey, params object[] messageArguments)
        : base(message)
    {
        MessageKey = messageKey;
        MessageArguments = messageArguments;
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        MessageArguments = Array.Empty<object>();
    }

    public DomainException(string message, Exception innerException, string messageKey, params object[] messageArguments)
        : base(message, innerException)
    {
        MessageKey = messageKey;
        MessageArguments = messageArguments;
    }
}

// ==================== Specific Domain Exceptions ====================
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base(
            $"{entityName} with key '{key}' was not found",
            "Error.EntityNotFound",
            entityName,
            key)
    { }
}

public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred", "ValidationFailed")
    {
        Errors = errors;
    }

    public ValidationException(string field, string error)
        : base($"Validation failed for field '{field}': {error}", "ValidationFailed")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }
}

public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleViolationException(string ruleName, string message)
        : base(
            $"Business rule '{ruleName}' violated: {message}",
            "Error.BusinessRuleViolation",
            ruleName,
            message)
    {
        RuleName = ruleName;
    }
}

public class ConcurrencyException : DomainException
{
    public ConcurrencyException(string entityName)
        : base(
            $"Concurrency conflict occurred for {entityName}",
            "Error.ConcurrencyConflict",
            entityName)
    { }
}

public class UnauthorizedAccessException : DomainException
{
    public UnauthorizedAccessException(string action, string resource)
        : base(
            $"Unauthorized access to perform '{action}' on '{resource}'",
            "Error.UnauthorizedResourceAccess",
            action,
            resource)
    { }
}

public class InvalidStateTransitionException : DomainException
{
    public InvalidStateTransitionException(string currentState, string targetState)
        : base(
            $"Cannot transition from '{currentState}' to '{targetState}'",
            "Error.InvalidStateTransition",
            currentState,
            targetState)
    { }
}
