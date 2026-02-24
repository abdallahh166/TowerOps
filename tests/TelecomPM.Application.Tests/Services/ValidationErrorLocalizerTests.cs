using FluentAssertions;
using System.Globalization;
using TelecomPm.Api.Localization;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class ValidationErrorLocalizerTests
{
    [Fact]
    public void Localize_ShouldTranslateFieldAndCommonMessages_WhenArabicCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-EG");
            CultureInfo.CurrentUICulture = new CultureInfo("ar-EG");

            var text = new LocalizedTextService();
            var sut = new ValidationErrorLocalizer(text);

            var input = new Dictionary<string, string[]>
            {
                ["Email"] =
                [
                    "'Email' must not be empty.",
                    "'Email' is not in the correct format."
                ],
                ["Password"] =
                [
                    "The length of 'Password' must be at least 8 characters. You entered 4 characters."
                ]
            };

            var localized = sut.Localize(input);

            localized.Keys.Should().Contain("البريد الإلكتروني");
            localized["البريد الإلكتروني"].Should().Contain("البريد الإلكتروني مطلوب.");
            localized["البريد الإلكتروني"].Should().Contain("تنسيق البريد الإلكتروني غير صحيح.");

            localized.Keys.Should().Contain("كلمة المرور");
            localized["كلمة المرور"][0].Should().Be("يجب أن يكون كلمة المرور على الأقل 8 أحرف.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
