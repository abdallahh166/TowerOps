namespace TelecomPM.Application.Common.Validation;

using FluentValidation.Resources;

/// <summary>
/// Central FluentValidation language manager for TelecomPM validation messages.
/// Keeps en/ar messages consistent at source instead of post-processing only.
/// </summary>
public sealed class TelecomPmLanguageManager : LanguageManager
{
    public TelecomPmLanguageManager()
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
        AddTranslation(culture, "NotEmptyValidator", "'{PropertyName}' مطلوب.");
        AddTranslation(culture, "NotNullValidator", "'{PropertyName}' مطلوب.");
        AddTranslation(culture, "EmailValidator", "تنسيق '{PropertyName}' غير صحيح.");
        AddTranslation(culture, "MinimumLengthValidator", "يجب أن يكون '{PropertyName}' على الأقل {MinLength} أحرف.");
        AddTranslation(culture, "MaximumLengthValidator", "يجب ألا يتجاوز '{PropertyName}' عدد {MaxLength} أحرف.");
        AddTranslation(culture, "LengthValidator", "يجب أن يكون طول '{PropertyName}' بين {MinLength} و {MaxLength}.");
        AddTranslation(culture, "InclusiveBetweenValidator", "يجب أن يكون '{PropertyName}' بين {From} و {To}.");
        AddTranslation(culture, "GreaterThanOrEqualValidator", "يجب أن يكون '{PropertyName}' أكبر من أو يساوي {ComparisonValue}.");
        AddTranslation(culture, "GreaterThanValidator", "يجب أن يكون '{PropertyName}' أكبر من {ComparisonValue}.");
        AddTranslation(culture, "LessThanOrEqualValidator", "يجب أن يكون '{PropertyName}' أقل من أو يساوي {ComparisonValue}.");
        AddTranslation(culture, "EqualValidator", "يجب أن يكون '{PropertyName}' مساويًا لـ '{ComparisonValue}'.");
    }
}
