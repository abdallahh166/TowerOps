using FluentAssertions;
using System.Globalization;
using TowerOps.Api.Localization;
using Xunit;

namespace TowerOps.Application.Tests.Services;

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

            localized.Keys.Should().Contain("Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ");
            localized["Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ"].Should().Contain("Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ Ù…Ø·Ù„ÙˆØ¨.");
            localized["Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ"].Should().Contain("ØªÙ†Ø³ÙŠÙ‚ Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ ØºÙŠØ± ØµØ­ÙŠØ­.");

            localized.Keys.Should().Contain("ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ±");
            localized["ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ±"][0].Should().Be("ÙŠØ¬Ø¨ Ø£Ù† ÙŠÙƒÙˆÙ† ÙƒÙ„Ù…Ø© Ø§Ù„Ù…Ø±ÙˆØ± Ø¹Ù„Ù‰ Ø§Ù„Ø£Ù‚Ù„ 8 Ø£Ø­Ø±Ù.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
