using FluentAssertions;
using System.Globalization;
using TelecomPm.Api.Localization;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class LocalizedTextServiceTests
{
    [Fact]
    public void Get_ShouldReturnArabicText_WhenCurrentCultureIsArabic()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-EG");
            CultureInfo.CurrentUICulture = new CultureInfo("ar-EG");

            var sut = new LocalizedTextService();
            var text = sut.Get("RequestFailed", "Request failed");

            text.Should().Be("فشل تنفيذ الطلب");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact]
    public void TranslateMessage_ShouldMapKnownMessage_WhenCurrentCultureIsArabic()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-EG");
            CultureInfo.CurrentUICulture = new CultureInfo("ar-EG");

            var sut = new LocalizedTextService();
            var translated = sut.TranslateMessage("Visit not found.");

            translated.Should().Be("الزيارة غير موجودة.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact]
    public void Get_ShouldFormatMessageTemplate_WhenArgumentsProvided()
    {
        var sut = new LocalizedTextService();

        var message = sut.Get("Error.EntityNotFound", null, "Visit", "123");

        message.Should().Be("Visit with key '123' was not found");
    }
}
