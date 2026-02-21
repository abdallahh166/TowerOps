using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Commands.Imports;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportSiteRadioData;

public record ImportSiteRadioDataCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportSiteRadioDataCommandValidator : AbstractValidator<ImportSiteRadioDataCommand>
{
    public ImportSiteRadioDataCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportSiteRadioDataCommandHandler : IRequestHandler<ImportSiteRadioDataCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportSiteRadioDataCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportSiteRadioDataCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportSiteDataResult();

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "Site Radio Data");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'Site Radio Data' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var trackedSites = new Dictionary<Guid, Site>();
        var groupedSectors = new Dictionary<Guid, List<SectorInfo>>();

        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var siteCode = ImportExcelSupport.GetCellText(row, columnMap, "Short Code", "ShortCode", "Site code");
            var key = ImportExcelSupport.NormalizeSiteKey(siteCode);
            if (string.IsNullOrWhiteSpace(key) || !siteLookup.TryGetValue(key, out var siteId))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: site '{key}' not found.");
                continue;
            }

            var sectorNumber = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "Sector number")) ?? 1;
            var azimuth = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "Azimuth")) ?? 0;
            var hba = ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "HBA(m)")) ?? 0;
            var antennaType = ImportExcelSupport.GetCellText(row, columnMap, "Antenna Type");
            var sectorBand = ImportExcelSupport.GetCellText(row, columnMap, "Sector Technology");
            var feederSize = ImportExcelSupport.GetCellText(row, columnMap, "Feeder Size / Cable Type");
            var feederLength = ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Feeder Length"));
            var rru = ImportExcelSupport.GetCellText(row, columnMap, "RRU/RF/RRH Solution");
            var electricalTilt = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "Elect Tilt"));
            var mechanicalTilt = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "Bracket Tilt"));

            try
            {
                var sector = SectorInfo.Create(
                    sectorNumber,
                    ImportExcelSupport.ParseTechnologyFromBand(sectorBand),
                    azimuth,
                    hba,
                    string.IsNullOrWhiteSpace(antennaType) ? "Unknown" : antennaType);

                sector.SetDeploymentDetails(sectorBand, sectorBand, rru, feederSize, feederLength, antennaType);
                sector.SetTilt(electricalTilt, mechanicalTilt);

                if (!groupedSectors.TryGetValue(siteId, out var sectorList))
                {
                    sectorList = new List<SectorInfo>();
                    groupedSectors[siteId] = sectorList;
                }

                sectorList.Add(sector);
                result.ImportedCount++;
            }
            catch (Exception ex)
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: {ex.Message}");
            }
        }

        foreach (var (siteId, sectors) in groupedSectors)
        {
            var site = await GetTrackedSiteAsync(siteId, trackedSites, cancellationToken);
            if (site is null)
                continue;

            var radio = SiteRadioEquipment.Create(site.Id);

            var has2G = sectors.Any(s => s.Technology == Technology.TwoG);
            var has3G = sectors.Any(s => s.Technology == Technology.ThreeG);
            var has4G = sectors.Any(s => s.Technology == Technology.FourG);

            if (has2G)
                radio.Enable2G(site.RadioEquipment?.BTSVendor ?? BTSVendor.NSN, site.RadioEquipment?.BTSType ?? "BTS", site.RadioEquipment?.BTSCount ?? 1, site.RadioEquipment?.TwoGModulesCount ?? 1);

            if (has3G)
                radio.Enable3G(site.RadioEquipment?.NodeBVendor ?? BTSVendor.Huawei, site.RadioEquipment?.NodeBType ?? "NodeB", site.RadioEquipment?.ThreeGRadioModules ?? 1, site.RadioEquipment?.ThreeGTransmissionModules ?? 1);

            if (has4G)
                radio.Enable4G(Math.Max(1, sectors.Count(s => s.Technology == Technology.FourG)));

            radio.SetSectors(sectors.OrderBy(s => s.SectorNumber).ThenBy(s => s.Azimuth).ToList());
            site.SetRadioEquipment(radio);
        }

        if (groupedSectors.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }

    private async Task<Site?> GetTrackedSiteAsync(Guid siteId, Dictionary<Guid, Site> trackedSites, CancellationToken cancellationToken)
    {
        if (trackedSites.TryGetValue(siteId, out var site))
            return site;

        site = await _siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is not null)
            trackedSites[siteId] = site;

        return site;
    }
}
