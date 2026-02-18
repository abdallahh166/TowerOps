using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Services;

public sealed class ValidationResult
{
    private readonly Dictionary<string, List<string>> _errors = new();
    private readonly Dictionary<string, List<string>> _warnings = new();

    public bool IsValid => !_errors.Any();
    public IReadOnlyDictionary<string, List<string>> Errors => _errors;
    public IReadOnlyDictionary<string, List<string>> Warnings => _warnings;

    public void AddError(string field, string message)
    {
        if (!_errors.ContainsKey(field))
            _errors[field] = new List<string>();

        _errors[field].Add(message);
    }

    public void AddWarning(string field, string message)
    {
        if (!_warnings.ContainsKey(field))
            _warnings[field] = new List<string>();

        _warnings[field].Add(message);
    }

    public void Merge(ValidationResult other)
    {
        foreach (var error in other.Errors)
        {
            foreach (var message in error.Value)
            {
                AddError(error.Key, message);
            }
        }

        foreach (var warning in other.Warnings)
        {
            foreach (var message in warning.Value)
            {
                AddWarning(warning.Key, message);
            }
        }
    }

    public ValidationException ToException()
    {
        var errorDict = _errors.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToArray()
        );

        return new ValidationException(errorDict);
    }
}
