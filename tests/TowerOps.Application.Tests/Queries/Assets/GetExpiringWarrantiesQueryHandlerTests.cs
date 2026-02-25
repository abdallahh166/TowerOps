using FluentAssertions;
using Moq;
using TowerOps.Application.Queries.Assets.GetExpiringWarranties;
using TowerOps.Domain.Entities.Assets;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Queries.Assets;

public class GetExpiringWarrantiesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUseRequestedDaysFilter()
    {
        var expectedDays = 60;
        var assets = new List<Asset>
        {
            Asset.Create("CAI001", AssetType.Battery, "Brand", "Model", "SN1", DateTime.UtcNow, DateTime.UtcNow.AddDays(10))
        };

        var repo = new Mock<IAssetRepository>();
        repo.Setup(r => r.GetExpiringWarrantiesAsNoTrackingAsync(expectedDays, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assets);

        var sut = new GetExpiringWarrantiesQueryHandler(repo.Object);
        var result = await sut.Handle(new GetExpiringWarrantiesQuery { Days = expectedDays }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        repo.Verify(r => r.GetExpiringWarrantiesAsNoTrackingAsync(expectedDays, It.IsAny<CancellationToken>()), Times.Once);
    }
}
