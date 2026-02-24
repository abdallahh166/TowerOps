using FluentAssertions;
using FluentValidation;
using FluentValidation.Resources;
using System.Globalization;
using TelecomPM.Application.Common.Validation;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class FluentValidationLanguageManagerTests
{
    [Fact]
    public void Validate_ShouldUseCustomEnglishMessage()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var originalLanguageManager = ValidatorOptions.Global.LanguageManager;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            ValidatorOptions.Global.LanguageManager = new TelecomPmLanguageManager();

            var validator = new SampleValidator();
            var result = validator.Validate(new SampleRequest(string.Empty));

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].ErrorMessage.Should().Be("'Email' is required.");
        }
        finally
        {
            ValidatorOptions.Global.LanguageManager = originalLanguageManager;
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact]
    public void LanguageManager_ShouldReturnArabicTranslation()
    {
        LanguageManager manager = new TelecomPmLanguageManager();

        var message = manager.GetString("NotEmptyValidator", new CultureInfo("ar-EG"));

        message.Should().Be("'{PropertyName}' مطلوب.");
    }

    private sealed record SampleRequest(string Email);

    private sealed class SampleValidator : AbstractValidator<SampleRequest>
    {
        public SampleValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
        }
    }
}
