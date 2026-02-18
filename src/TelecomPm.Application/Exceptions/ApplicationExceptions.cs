using System;
using System.Collections.Generic;

namespace TelecomPM.Application.Exceptions;

public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base($"Entity '{name}' with key '{key}' was not found") { }
}

public class ValidationException : ApplicationException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(message) { }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message) { }
}