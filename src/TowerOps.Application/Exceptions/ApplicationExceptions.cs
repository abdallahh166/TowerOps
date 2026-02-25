using System;
using System.Collections.Generic;

namespace TowerOps.Application.Exceptions;

public class ApplicationException : Exception
{
    public string? MessageKey { get; }
    public object[] MessageArguments { get; }

    public ApplicationException(string message) : base(message)
    {
        MessageArguments = Array.Empty<object>();
    }

    public ApplicationException(string message, string messageKey, params object[] messageArguments)
        : base(message)
    {
        MessageKey = messageKey;
        MessageArguments = messageArguments;
    }

    public ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
        MessageArguments = Array.Empty<object>();
    }

    public ApplicationException(string message, Exception innerException, string messageKey, params object[] messageArguments)
        : base(message, innerException)
    {
        MessageKey = messageKey;
        MessageArguments = messageArguments;
    }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base(
            $"Entity '{name}' with key '{key}' was not found",
            "Error.EntityNotFound",
            name,
            key)
    { }
}

public class ValidationException : ApplicationException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred", "ValidationFailed")
    {
        Errors = errors;
    }
}

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(message, "Error.UnauthorizedGeneric") { }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message, "Error.ConflictGeneric") { }
}
