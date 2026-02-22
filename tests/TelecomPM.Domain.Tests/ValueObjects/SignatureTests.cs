using FluentAssertions;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.ValueObjects;

public class SignatureTests
{
    [Fact]
    public void Create_WithInvalidBase64_ShouldThrow()
    {
        var act = () => Signature.Create("Signer", "ClientRep", "not-base64");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithOversizedPayload_ShouldThrow()
    {
        var bytes = new byte[151 * 1024];
        bytes[0] = 137; bytes[1] = 80; bytes[2] = 78; bytes[3] = 71;
        bytes[4] = 13; bytes[5] = 10; bytes[6] = 26; bytes[7] = 10;
        var base64 = Convert.ToBase64String(bytes);

        var act = () => Signature.Create("Signer", "ClientRep", base64);

        act.Should().Throw<DomainException>()
            .WithMessage("*150KB*");
    }
}
