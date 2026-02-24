using FluentAssertions;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.ValueObjects;

public class ContactValueObjectsTests
{
    [Fact]
    public void Email_Create_WithInvalidFormat_ShouldSetLocalizationKey()
    {
        Action act = () => Email.Create("invalid-email");

        var ex = act.Should().Throw<DomainException>().Which;
        ex.MessageKey.Should().Be("Email.InvalidFormat");
    }

    [Fact]
    public void PhoneNumber_Create_WithInvalidFormat_ShouldSetLocalizationKey()
    {
        Action act = () => PhoneNumber.Create("12345");

        var ex = act.Should().Throw<DomainException>().Which;
        ex.MessageKey.Should().Be("PhoneNumber.InvalidFormat");
    }
}
