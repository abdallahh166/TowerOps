using FluentAssertions;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.ValueObjects;

public class SiteCodeTests
{
    [Fact]
    public void Create_WithValidCode_ShouldParseOfficeAndSequence()
    {
        var sc = SiteCode.Create("TNT001");
        sc.Value.Should().Be("TNT001");
        sc.OfficeCode.Should().Be("TNT");
        sc.SequenceNumber.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("TN1")]
    [InlineData("TNTABC")]
    public void Create_WithInvalid_ShouldThrow(string code)
    {
        var act = () => SiteCode.Create(code);
        act.Should().Throw<DomainException>();
    }
}


