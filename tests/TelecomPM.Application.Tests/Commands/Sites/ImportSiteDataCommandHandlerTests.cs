using ClosedXML.Excel;
using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Common;
using TelecomPM.Application.Commands.Sites.CreateSite;
using TelecomPM.Application.Commands.Sites.ImportSiteData;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Sites;

public class ImportSiteDataCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRows_ShouldImportCount()
    {
        var officeRepo = new Mock<IOfficeRepository>();
        officeRepo.Setup(x => x.GetByCodeAsNoTrackingAsync("CAI", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Office.Create("CAI", "Cairo Office", "Cairo", Address.Create("Street", "Cairo", "Cairo")));

        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<CreateSiteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new SiteDetailDto { Id = Guid.NewGuid() }));

        var sut = new ImportSiteDataCommandHandler(officeRepo.Object, sender.Object);
        var command = new ImportSiteDataCommand { FileContent = BuildWorkbookBytes(("CAI001", "Site 1", "CAI", "R1", "SR1", "Macro", "30.1", "31.2")) };

        var result = await sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().Be(1);
        result.Value.SkippedCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithInvalidSiteCode_ShouldSkipAndAddError()
    {
        var officeRepo = new Mock<IOfficeRepository>();
        var sender = new Mock<ISender>();
        var sut = new ImportSiteDataCommandHandler(officeRepo.Object, sender.Object);

        var command = new ImportSiteDataCommand { FileContent = BuildWorkbookBytes(("BAD", "Site 1", "CAI", "R1", "SR1", "Macro", "30.1", "31.2")) };
        var result = await sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ImportedCount.Should().Be(0);
        result.Value.SkippedCount.Should().Be(1);
        result.Value.Errors.Should().ContainSingle(x => x.Contains("invalid SiteCode", StringComparison.OrdinalIgnoreCase));
        sender.Verify(x => x.Send(It.IsAny<CreateSiteCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static byte[] BuildWorkbookBytes(params (string SiteCode, string SiteName, string OfficeCode, string Region, string SubRegion, string SiteType, string Latitude, string Longitude)[] rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Sites");
        ws.Cell(1, 1).Value = "SiteCode";
        ws.Cell(1, 2).Value = "SiteName";
        ws.Cell(1, 3).Value = "OfficeCode";
        ws.Cell(1, 4).Value = "Region";
        ws.Cell(1, 5).Value = "SubRegion";
        ws.Cell(1, 6).Value = "SiteType";
        ws.Cell(1, 7).Value = "Latitude";
        ws.Cell(1, 8).Value = "Longitude";

        for (var i = 0; i < rows.Length; i++)
        {
            var row = i + 2;
            ws.Cell(row, 1).Value = rows[i].SiteCode;
            ws.Cell(row, 2).Value = rows[i].SiteName;
            ws.Cell(row, 3).Value = rows[i].OfficeCode;
            ws.Cell(row, 4).Value = rows[i].Region;
            ws.Cell(row, 5).Value = rows[i].SubRegion;
            ws.Cell(row, 6).Value = rows[i].SiteType;
            ws.Cell(row, 7).Value = rows[i].Latitude;
            ws.Cell(row, 8).Value = rows[i].Longitude;
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
