namespace TowerOps.Api.Localization;

public interface IValidationErrorLocalizer
{
    Dictionary<string, string[]> Localize(Dictionary<string, string[]> errors);
}
