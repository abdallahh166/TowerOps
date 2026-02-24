namespace TelecomPm.Api.Localization;

public interface ILocalizedTextService
{
    string Get(string key, string? fallback = null, params object[] formatArgs);
    string TranslateMessage(string message);
}
