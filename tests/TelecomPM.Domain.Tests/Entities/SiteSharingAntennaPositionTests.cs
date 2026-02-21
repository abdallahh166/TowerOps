using FluentAssertions;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Tests.Entities;

public class SiteSharingAntennaPositionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(180)]
    [InlineData(360)]
    public void Create_ShouldAcceptAzimuthRange_0To360(decimal azimuth)
    {
        var siteSharingId = Guid.NewGuid();

        var position = SharedAntennaPosition.Create(siteSharingId, "Radio", 1, azimuth, 10);

        position.Azimuth.Should().Be(azimuth);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(361)]
    public void Create_ShouldRejectAzimuthOutsideRange(decimal azimuth)
    {
        var siteSharingId = Guid.NewGuid();

        var act = () => SharedAntennaPosition.Create(siteSharingId, "Radio", 1, azimuth, 10);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    public void Create_ShouldRejectNonPositiveHba(decimal hbaMeters)
    {
        var siteSharingId = Guid.NewGuid();

        var act = () => SharedAntennaPosition.Create(siteSharingId, "TX", 1, 90, hbaMeters);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddAntennaPosition_ShouldEnforceMax8RadioAndMax9Tx()
    {
        var sharing = SiteSharing.Create(Guid.NewGuid());

        for (var i = 1; i <= 8; i++)
        {
            sharing.AddAntennaPosition(SharedAntennaPosition.Create(sharing.Id, "Radio", i, 10 + i, 5));
        }

        var addNinthRadio = () => sharing.AddAntennaPosition(
            SharedAntennaPosition.Create(sharing.Id, "Radio", 9, 100, 5));
        addNinthRadio.Should().Throw<DomainException>();

        for (var i = 1; i <= 9; i++)
        {
            sharing.AddAntennaPosition(SharedAntennaPosition.Create(sharing.Id, "TX", i, 200 + i, 6));
        }

        var addTenthTx = () => sharing.AddAntennaPosition(
            SharedAntennaPosition.Create(sharing.Id, "TX", 10, 300, 6));
        addTenthTx.Should().Throw<DomainException>();
    }
}
