namespace TelecomPM.Domain.Exceptions;

// ==================== Base Domain Exception ====================
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    
    public DomainException(string message, Exception innerException) 
        : base(message, innerException) { }
}

// ==================== Specific Domain Exceptions ====================
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found") { }
}

public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }

    public ValidationException(string field, string error)
        : base($"Validation failed for field '{field}': {error}")
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
        : base($"Business rule '{ruleName}' violated: {message}")
    {
        RuleName = ruleName;
    }
}

public class ConcurrencyException : DomainException
{
    public ConcurrencyException(string entityName)
        : base($"Concurrency conflict occurred for {entityName}") { }
}

public class UnauthorizedAccessException : DomainException
{
    public UnauthorizedAccessException(string action, string resource)
        : base($"Unauthorized access to perform '{action}' on '{resource}'") { }
}

public class InvalidStateTransitionException : DomainException
{
    public InvalidStateTransitionException(string currentState, string targetState)
        : base($"Cannot transition from '{currentState}' to '{targetState}'") { }
}
