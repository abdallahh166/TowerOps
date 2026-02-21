using FluentAssertions;
using TelecomPM.Domain.Entities.BatteryDischargeTests;
using TelecomPM.Domain.Exceptions;
using Xunit;

namespace TelecomPM.Domain.Tests.Entities;

public class BatteryDischargeTestTests
{
    [Fact]
    public void Create_WithRequiredFields_ShouldInitializeAggregate()
    {
        var siteId = Guid.NewGuid();
        var testDateUtc = DateTime.UtcNow;

        var bdt = BatteryDischargeTest.Create(siteId, "3564DE", testDateUtc);

        bdt.Id.Should().NotBe(Guid.Empty);
        bdt.SiteId.Should().Be(siteId);
        bdt.SiteCode.Should().Be("3564DE");
        bdt.TestDateUtc.Should().Be(testDateUtc);
    }

    [Fact]
    public void Create_WithoutSiteCode_ShouldThrowDomainException()
    {
        var act = () => BatteryDischargeTest.Create(Guid.NewGuid(), string.Empty, DateTime.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("*SiteCode is required*");
    }
}
