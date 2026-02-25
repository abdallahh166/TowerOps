namespace TowerOps.Application.Common.Validation;

using FluentValidation.Resources;

/// <summary>
/// Central FluentValidation language manager for TowerOps validation messages.
/// Keeps en/ar messages consistent at source instead of post-processing only.
/// </summary>
public sealed class TowerOpsLanguageManager : LanguageManager
{
    public TowerOpsLanguageManager()
    {
        Enabled = true;

        ApplyEnglishTranslations("en");
        ApplyEnglishTranslations("en-US");
        ApplyArabicTranslations("ar");
        ApplyArabicTranslations("ar-EG");
    }

    private void ApplyEnglishTranslations(string culture)
    {
        AddTranslation(culture, "NotEmptyValidator", "'{PropertyName}' is required.");
        AddTranslation(culture, "NotNullValidator", "'{PropertyName}' is required.");
        AddTranslation(culture, "EmailValidator", "'{PropertyName}' has invalid format.");
        AddTranslation(culture, "MinimumLengthValidator", "'{PropertyName}' must be at least {MinLength} characters.");
        AddTranslation(culture, "MaximumLengthValidator", "'{PropertyName}' must be {MaxLength} characters or fewer.");
        AddTranslation(culture, "LengthValidator", "'{PropertyName}' must be between {MinLength} and {MaxLength} characters.");
        AddTranslation(culture, "InclusiveBetweenValidator", "'{PropertyName}' must be between {From} and {To}.");
        AddTranslation(culture, "GreaterThanOrEqualValidator", "'{PropertyName}' must be greater than or equal to {ComparisonValue}.");
        AddTranslation(culture, "GreaterThanValidator", "'{PropertyName}' must be greater than {ComparisonValue}.");
        AddTranslation(culture, "LessThanOrEqualValidator", "'{PropertyName}' must be less than or equal to {ComparisonValue}.");
        AddTranslation(culture, "EqualValidator", "'{PropertyName}' must be equal to '{ComparisonValue}'.");
    }

    private void ApplyArabicTranslations(string culture)
    {
        AddTranslation(culture, "NotEmptyValidator", "'{PropertyName}' Ù…Ø·Ù„ÙˆØ¨.");
        AddTranslation(culture, "NotNullValidator", "'{PropertyName}' Ù…Ø·Ù„ÙˆØ¨.");
        AddTranslation(culture, "EmailValidator", "ØªÙ†Ø³ÙŠÙ‚ '{PropertyName}' ØºÙŠØ± ØµØ­ÙŠØ­.");
        AddTranslation(culture, "MinimumLengthValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† '{PropertyName}' Ø¹Ù„Ù‰ Ø§Ù„Ø£Ù‚Ù„ {MinLength} Ø£Ø­Ø±Ù.");
        AddTranslation(culture, "MaximumLengthValidator", "ÙŠØ¬Ø¨ Ø£Ù„Ø§ ÙŠØªØ¬Ø§ÙˆØ² '{PropertyName}' Ø¹Ø¯Ø¯ {MaxLength} Ø£Ø­Ø±Ù.");
        AddTranslation(culture, "LengthValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† Ø·ÙˆÙ„ '{PropertyName}' Ø¨ÙŠÙ† {MinLength} Ùˆ {MaxLength}.");
        AddTranslation(culture, "InclusiveBetweenValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† '{PropertyName}' Ø¨ÙŠÙ† {From} Ùˆ {To}.");
        AddTranslation(culture, "GreaterThanOrEqualValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† '{PropertyName}' Ø£ÙƒØ¨Ø± Ù…Ù† Ø£Ùˆ ÙŠØ³Ø§ÙˆÙŠ {ComparisonValue}.");
        AddTranslation(culture, "GreaterThanValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† '{PropertyName}' Ø£ÙƒØ¨Ø± Ù…Ù† {ComparisonValue}.");
        AddTranslation(culture, "LessThanOrEqualValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† '{PropertyName}' Ø£Ù‚Ù„ Ù…Ù† Ø£Ùˆ ÙŠØ³Ø§ÙˆÙŠ {ComparisonValue}.");
        AddTranslation(culture, "EqualValidator", "ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† '{PropertyName}' Ù…Ø³Ø§ÙˆÙŠÙ‹Ø§ Ù„Ù€ '{ComparisonValue}'.");
    }
}
