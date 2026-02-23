using FluentAssertions;
using TelecomPM.Domain.Entities.UnusedAssets;
using TelecomPM.Domain.Exceptions;
using Xunit;

namespace TelecomPM.Domain.Tests.Entities;

public class UnusedAssetTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeAggregate()
    {
        var siteId = Guid.NewGuid();
        var visitId = Guid.NewGuid();
        var nowUtc = DateTime.UtcNow;

        var asset = UnusedAsset.Create(
            siteId,
            visitId,
            "Rectifier Module",
            2,
            "pcs",
            nowUtc,
            "Returned from site");

        asset.Id.Should().NotBe(Guid.Empty);
        asset.SiteId.Should().Be(siteId);
        asset.VisitId.Should().Be(visitId);
        asset.AssetName.Should().Be("Rectifier Module");
        asset.Quantity.Should().Be(2);
        asset.Unit.Should().Be("pcs");
        asset.Notes.Should().Be("Returned from site");
        asset.RecordedAtUtc.Should().Be(nowUtc);
    }

    [Fact]
    public void Create_WithNonPositiveQuantity_ShouldThrowDomainException()
    {
        var act = () => UnusedAsset.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Battery",
            0,
            "pcs",
            DateTime.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("*Quantity must be greater than zero*");
    }
}
