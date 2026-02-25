using System.Text.RegularExpressions;

namespace TowerOps.Api.Localization;

public sealed partial class ValidationErrorLocalizer : IValidationErrorLocalizer
{
    private readonly ILocalizedTextService _localizedText;

    private static readonly IReadOnlyDictionary<string, string> FieldKeyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Email"] = "Field.Email",
        ["Password"] = "Field.Password",
        ["CurrentPassword"] = "Field.CurrentPassword",
        ["NewPassword"] = "Field.NewPassword",
        ["ConfirmPassword"] = "Field.ConfirmPassword",
        ["PhoneNumber"] = "Field.PhoneNumber",
        ["VisitId"] = "Field.VisitId",
        ["SiteId"] = "Field.SiteId",
        ["SiteCode"] = "Field.SiteCode",
        ["OfficeId"] = "Field.OfficeId",
        ["EngineerId"] = "Field.EngineerId",
        ["WorkOrderId"] = "Field.WorkOrderId",
        ["Id"] = "Field.Id",
        ["Name"] = "Field.Name",
        ["Role"] = "Field.Role"
    };

    public ValidationErrorLocalizer(ILocalizedTextService localizedText)
    {
        _localizedText = localizedText;
    }

    public Dictionary<string, string[]> Localize(Dictionary<string, string[]> errors)
    {
        if (errors.Count == 0)
            return errors;

        var localized = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var (field, messages) in errors)
        {
            var displayField = LocalizeField(field);
            var localizedMessages = messages
                .Select(message => LocalizeMessage(message, field))
                .ToArray();

            localized[displayField] = localizedMessages;
        }

        return localized;
    }

    private string LocalizeField(string field)
    {
        if (FieldKeyMap.TryGetValue(field, out var key))
            return _localizedText.Get(key, field);

        return field;
    }

    private string LocalizeMessage(string message, string originalField)
    {
        if (string.IsNullOrWhiteSpace(message))
            return message;

        var notEmptyMatch = NotEmptyRegex().Match(message);
        if (notEmptyMatch.Success)
        {
            var field = notEmptyMatch.Groups["field"].Value;
            var localizedField = LocalizeFieldOrFallback(field, originalField);
            return string.Format(
                _localizedText.Get("Validation.Required", "{0} is required."),
                localizedField);
        }

        var formatMatch = InvalidFormatRegex().Match(message);
        if (formatMatch.Success)
        {
            var field = formatMatch.Groups["field"].Value;
            var localizedField = LocalizeFieldOrFallback(field, originalField);
            return string.Format(
                _localizedText.Get("Validation.InvalidFormat", "{0} has invalid format."),
                localizedField);
        }

        var minLengthMatch = MinLengthRegex().Match(message);
        if (minLengthMatch.Success)
        {
            var field = minLengthMatch.Groups["field"].Value;
            var min = minLengthMatch.Groups["min"].Value;
            var localizedField = LocalizeFieldOrFallback(field, originalField);
            return string.Format(
                _localizedText.Get("Validation.MinLength", "{0} must be at least {1} characters."),
                localizedField,
                min);
        }

        var maxLengthMatch = MaxLengthRegex().Match(message);
        if (maxLengthMatch.Success)
        {
            var field = maxLengthMatch.Groups["field"].Value;
            var max = maxLengthMatch.Groups["max"].Value;
            var localizedField = LocalizeFieldOrFallback(field, originalField);
            return string.Format(
                _localizedText.Get("Validation.MaxLength", "{0} must be {1} characters or fewer."),
                localizedField,
                max);
        }

        var translated = _localizedText.TranslateMessage(message);
        return translated;
    }

    private string LocalizeFieldOrFallback(string parsedField, string fallbackField)
    {
        if (!string.IsNullOrWhiteSpace(parsedField))
            return LocalizeField(parsedField);

        return LocalizeField(fallbackField);
    }

    [GeneratedRegex("^'(?<field>.+?)' must not be empty\\.$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex NotEmptyRegex();

    [GeneratedRegex("^'(?<field>.+?)' is not in the correct format\\.$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex InvalidFormatRegex();

    [GeneratedRegex("^The length of '(?<field>.+?)' must be at least (?<min>\\d+) characters\\..*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MinLengthRegex();

    [GeneratedRegex("^The length of '(?<field>.+?)' must be (?<max>\\d+) characters or fewer\\..*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MaxLengthRegex();
}
