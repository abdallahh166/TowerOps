namespace TelecomPm.Api.Localization;

public interface IValidationErrorLocalizer
{
    Dictionary<string, string[]> Localize(Dictionary<string, string[]> errors);
}
