using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Reports.ExportDataCollection;

public record ExportDataCollectionCommand : ICommand<byte[]>
{
    public string? OfficeCode { get; init; }
}

public class ExportDataCollectionCommandValidator : AbstractValidator<ExportDataCollectionCommand>
{
    public ExportDataCollectionCommandValidator()
    {
        RuleFor(x => x.OfficeCode)
            .MaximumLength(20);
    }
}

public sealed class ExportDataCollectionCommandHandler : IRequestHandler<ExportDataCollectionCommand, Result<byte[]>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IOfficeRepository _officeRepository;

    public ExportDataCollectionCommandHandler(ISiteRepository siteRepository, IOfficeRepository officeRepository)
    {
        _siteRepository = siteRepository;
        _officeRepository = officeRepository;
    }

    public async Task<Result<byte[]>> Handle(ExportDataCollectionCommand request, CancellationToken cancellationToken)
    {
        var sites = await LoadSitesAsync(request.OfficeCode, cancellationToken);

        using var workbook = new XLWorkbook();
        BuildSiteAssetsSheet(workbook, sites);
        BuildPowerDataSheet(workbook, sites);
        BuildSiteRadioDataSheet(workbook, sites);
        BuildSiteTxDataSheet(workbook, sites);
        BuildSiteSharingDataSheet(workbook, sites);
        BuildRfStatusSheet(workbook, sites);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Result.Success(stream.ToArray());
    }

    private async Task<IReadOnlyList<Site>> LoadSitesAsync(string? officeCode, CancellationToken cancellationToken)
    {
        IReadOnlyList<Site> baseSites;

        if (!string.IsNullOrWhiteSpace(officeCode))
        {
            var office = await _officeRepository.GetByCodeAsNoTrackingAsync(officeCode, cancellationToken);
            if (office is null)
                return Array.Empty<Site>();

            baseSites = await _siteRepository.GetByOfficeIdAsNoTrackingAsync(office.Id, cancellationToken);
        }
        else
        {
            baseSites = await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken);
        }

        var hydrated = new List<Site>(baseSites.Count);
        foreach (var site in baseSites)
        {
            var full = await _siteRepository.GetByIdAsNoTrackingAsync(site.Id, cancellationToken);
            if (full is not null)
                hydrated.Add(full);
        }

        return hydrated;
    }

    private static void BuildSiteAssetsSheet(XLWorkbook workbook, IReadOnlyList<Site> sites)
    {
        var ws = workbook.AddWorksheet("Site Assets Data Count");
        ws.Cell(1, 1).Value = "ShortCode";
        ws.Cell(1, 2).Value = "Site OMC Name";
        ws.Cell(1, 3).Value = "Site Code";
        ws.Cell(1, 4).Value = "BSC Name";
        ws.Cell(1, 5).Value = "Subcontractor";
        ws.Cell(1, 6).Value = "Maintenance Area";
        ws.Cell(1, 7).Value = "Region";
        ws.Cell(1, 8).Value = "Subregion";
        ws.Cell(1, 9).Value = "On / Off  Air";
        ws.Cell(1, 10).Value = "Site Coordinates X, Y";

        var row = 2;
        foreach (var site in sites)
        {
            ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
            ws.Cell(row, 2).Value = site.OMCName;
            ws.Cell(row, 3).Value = site.SiteCode.Value;
            ws.Cell(row, 4).Value = site.BSCName;
            ws.Cell(row, 5).Value = site.Subcontractor;
            ws.Cell(row, 6).Value = site.MaintenanceArea;
            ws.Cell(row, 7).Value = site.Region;
            ws.Cell(row, 8).Value = site.SubRegion;
            ws.Cell(row, 9).Value = site.Status == SiteStatus.OnAir ? "On Air" : "Off Air";
            ws.Cell(row, 10).Value = $"{site.Coordinates.Latitude},{site.Coordinates.Longitude}";
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildPowerDataSheet(XLWorkbook workbook, IReadOnlyList<Site> sites)
    {
        var ws = workbook.AddWorksheet("Power Data");
        ws.Cell(1, 1).Value = "Site Code";
        ws.Cell(1, 2).Value = "OEG Site Name";
        ws.Cell(1, 3).Value = "TE Name";
        ws.Cell(1, 4).Value = "Site Type";
        ws.Cell(1, 5).Value = "Rectifier Type";
        ws.Cell(1, 6).Value = "No of Modules";
        ws.Cell(1, 7).Value = "Routers";
        ws.Cell(1, 8).Value = "Modem";
        ws.Cell(1, 9).Value = "Battery Type";

        var row = 2;
        foreach (var site in sites)
        {
            var power = site.PowerSystem;
            ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
            ws.Cell(row, 2).Value = site.Name;
            ws.Cell(row, 3).Value = site.TelecomEgyptName;
            ws.Cell(row, 4).Value = site.SiteType.ToString();
            ws.Cell(row, 5).Value = power?.RectifierBrandRaw ?? power?.RectifierBrand.ToString();
            ws.Cell(row, 6).Value = power?.RectifierModulesCount;
            ws.Cell(row, 7).Value = power?.RouterCount;
            ws.Cell(row, 8).Value = power?.ModemCount;
            ws.Cell(row, 9).Value = power?.BatteryBrand ?? power?.BatteryType.ToString();
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildSiteRadioDataSheet(XLWorkbook workbook, IReadOnlyList<Site> sites)
    {
        var ws = workbook.AddWorksheet("Site Radio Data");
        ws.Cell(1, 1).Value = "Short Code";
        ws.Cell(1, 2).Value = "Name";
        ws.Cell(1, 3).Value = "Sector Technology";
        ws.Cell(1, 4).Value = "Sector number";
        ws.Cell(1, 5).Value = "Azimuth";
        ws.Cell(1, 6).Value = "Antenna Type";
        ws.Cell(1, 7).Value = "HBA(m)";

        var row = 2;
        foreach (var site in sites)
        {
            if (site.RadioEquipment?.Sectors is null || site.RadioEquipment.Sectors.Count == 0)
            {
                ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
                ws.Cell(row, 2).Value = site.Name;
                row++;
                continue;
            }

            foreach (var sector in site.RadioEquipment.Sectors)
            {
                ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
                ws.Cell(row, 2).Value = site.Name;
                ws.Cell(row, 3).Value = sector.BandLabel ?? sector.SectorTechnology;
                ws.Cell(row, 4).Value = sector.SectorNumber;
                ws.Cell(row, 5).Value = sector.Azimuth;
                ws.Cell(row, 6).Value = sector.AntennaType;
                ws.Cell(row, 7).Value = sector.HeightAboveBase;
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildSiteTxDataSheet(XLWorkbook workbook, IReadOnlyList<Site> sites)
    {
        var ws = workbook.AddWorksheet("Site TX Data");
        ws.Cell(1, 1).Value = "Short Code";
        ws.Cell(1, 2).Value = "Directions Site Code";
        ws.Cell(1, 3).Value = "Band";
        ws.Cell(1, 4).Value = "Configuration";
        ws.Cell(1, 5).Value = "Modulation";
        ws.Cell(1, 6).Value = "Capacity [Mb/s]";
        ws.Cell(1, 7).Value = "Tx Frequency [KHz]";
        ws.Cell(1, 8).Value = "Rx Frequency [KHz]";

        var row = 2;
        foreach (var site in sites)
        {
            if (site.Transmission?.MWLinks is null || site.Transmission.MWLinks.Count == 0)
            {
                ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
                row++;
                continue;
            }

            foreach (var link in site.Transmission.MWLinks)
            {
                ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
                ws.Cell(row, 2).Value = link.OppositeSite;
                ws.Cell(row, 3).Value = link.FrequencyBand;
                ws.Cell(row, 4).Value = link.Configuration;
                ws.Cell(row, 5).Value = link.Modulation;
                ws.Cell(row, 6).Value = link.CapacityMbps;
                ws.Cell(row, 7).Value = link.TxFrequencyKHz;
                ws.Cell(row, 8).Value = link.RxFrequencyKHz;
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildSiteSharingDataSheet(XLWorkbook workbook, IReadOnlyList<Site> sites)
    {
        var ws = workbook.AddWorksheet("Site Sharing Data");
        ws.Cell(1, 1).Value = "Short Code";
        ws.Cell(1, 2).Value = "Site Host";
        ws.Cell(1, 3).Value = "Host Code";
        ws.Cell(1, 4).Value = "Site Guests";
        ws.Cell(1, 5).Value = "TX Enclosure";
        ws.Cell(1, 6).Value = "Sharing Count Radio Antenna";
        ws.Cell(1, 7).Value = "Sharing Count Tx Antenna";

        var row = 2;
        foreach (var site in sites)
        {
            ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
            ws.Cell(row, 2).Value = site.SharingInfo?.HostOperator;
            ws.Cell(row, 3).Value = site.SharingInfo?.HostSiteCode;
            ws.Cell(row, 4).Value = site.SharingInfo is null ? null : string.Join(",", site.SharingInfo.GuestOperators);
            ws.Cell(row, 5).Value = site.SharingInfo?.TxEnclosureType;
            ws.Cell(row, 6).Value = site.SharingInfo?.SharingRadioAntennaCount;
            ws.Cell(row, 7).Value = site.SharingInfo?.SharingTxAntennaCount;
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildRfStatusSheet(XLWorkbook workbook, IReadOnlyList<Site> sites)
    {
        var ws = workbook.AddWorksheet("RF Status");
        ws.Cell(1, 1).Value = "Site code";
        ws.Cell(1, 2).Value = "Total RF Count";
        ws.Cell(1, 3).Value = "RF On Tower Count";
        ws.Cell(1, 4).Value = "RF On Ground Count";
        ws.Cell(1, 5).Value = "RF Sector Carry Count";
        ws.Cell(1, 6).Value = "Band For RF On Tower";
        ws.Cell(1, 7).Value = "Band For RF On Ground";
        ws.Cell(1, 8).Value = "comment";

        var row = 2;
        foreach (var site in sites)
        {
            ws.Cell(row, 1).Value = site.SiteCode.ShortCode ?? site.SiteCode.Value;
            ws.Cell(row, 2).Value = site.RFStatus?.TotalRFCount;
            ws.Cell(row, 3).Value = site.RFStatus?.RFOnTowerCount;
            ws.Cell(row, 4).Value = site.RFStatus?.RFOnGroundCount;
            ws.Cell(row, 5).Value = site.RFStatus?.RFSectorCarryCount;
            ws.Cell(row, 6).Value = site.RFStatus?.BandForRFOnTower;
            ws.Cell(row, 7).Value = site.RFStatus?.BandForRFOnGround;
            ws.Cell(row, 8).Value = site.RFStatus?.Notes;
            row++;
        }

        ws.Columns().AdjustToContents();
    }
}
