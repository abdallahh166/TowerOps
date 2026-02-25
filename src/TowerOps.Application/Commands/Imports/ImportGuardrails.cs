using ClosedXML.Excel;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sites;

namespace TowerOps.Application.Commands.Imports;

internal sealed record ImportExecutionOptions(
    bool SkipInvalidRows,
    int MaxRows,
    int MaxFileSizeBytes,
    string DefaultDateFormat)
{
    public const int DefaultMaxRows = 5000;
    public const int DefaultMaxFileSizeBytes = 10 * 1024 * 1024;
    public const string DefaultDateFormatValue = "dd/MM/yyyy";
}

internal static class ImportGuardrails
{
    public static async Task<ImportExecutionOptions> ResolveOptionsAsync(
        ISystemSettingsService? systemSettingsService,
        CancellationToken cancellationToken)
    {
        if (systemSettingsService is null)
        {
            return new ImportExecutionOptions(
                SkipInvalidRows: true,
                MaxRows: ImportExecutionOptions.DefaultMaxRows,
                MaxFileSizeBytes: ImportExecutionOptions.DefaultMaxFileSizeBytes,
                DefaultDateFormat: ImportExecutionOptions.DefaultDateFormatValue);
        }

        var skipInvalidRows = await systemSettingsService.GetAsync(
            "Import:SkipInvalidRows",
            true,
            cancellationToken);

        var maxRows = await systemSettingsService.GetAsync(
            "Import:MaxRows",
            ImportExecutionOptions.DefaultMaxRows,
            cancellationToken);

        var maxFileSizeBytes = await systemSettingsService.GetAsync(
            "Import:MaxFileSizeBytes",
            ImportExecutionOptions.DefaultMaxFileSizeBytes,
            cancellationToken);

        var defaultDateFormat = await systemSettingsService.GetAsync(
            "Import:DefaultDateFormat",
            ImportExecutionOptions.DefaultDateFormatValue,
            cancellationToken);

        return new ImportExecutionOptions(
            SkipInvalidRows: skipInvalidRows,
            MaxRows: maxRows <= 0 ? ImportExecutionOptions.DefaultMaxRows : maxRows,
            MaxFileSizeBytes: maxFileSizeBytes <= 0 ? ImportExecutionOptions.DefaultMaxFileSizeBytes : maxFileSizeBytes,
            DefaultDateFormat: string.IsNullOrWhiteSpace(defaultDateFormat)
                ? ImportExecutionOptions.DefaultDateFormatValue
                : defaultDateFormat);
    }

    public static string? ValidateExcelPayload(byte[] fileContent, ImportExecutionOptions options)
    {
        if (fileContent is null || fileContent.Length == 0)
            return "Excel file content is required.";

        if (fileContent.Length > options.MaxFileSizeBytes)
            return $"Import file size {fileContent.Length} bytes exceeds configured maximum {options.MaxFileSizeBytes} bytes.";

        if (!LooksLikeExcel(fileContent))
            return "Unsupported file type. Only Excel files (.xlsx/.xlsm/.xls) are allowed.";

        return null;
    }

    public static int CountNonEmptyDataRows(XLWorkbook workbook)
    {
        var maxRowsAcrossSheets = 0;
        foreach (var worksheet in workbook.Worksheets)
        {
            var sheetRows = CountNonEmptyDataRows(worksheet);
            if (sheetRows > maxRowsAcrossSheets)
                maxRowsAcrossSheets = sheetRows;
        }

        return maxRowsAcrossSheets;
    }

    public static int CountNonEmptyDataRows(IXLWorksheet worksheet)
    {
        var nonEmptyRows = worksheet.RowsUsed(XLCellsUsedOptions.Contents)
            .Count(row => !row.IsEmpty(XLCellsUsedOptions.Contents));

        if (nonEmptyRows <= 0)
            return 0;

        // Exclude header row.
        return Math.Max(0, nonEmptyRows - 1);
    }

    public static Result<ImportSiteDataResult>? ValidateRowLimit(int nonEmptyDataRows, ImportExecutionOptions options)
    {
        if (nonEmptyDataRows <= options.MaxRows)
            return null;

        return Result.Failure<ImportSiteDataResult>(
            $"Import row count {nonEmptyDataRows} exceeds configured maximum {options.MaxRows}.");
    }

    public static Result<ImportSiteDataResult>? EnforceSkipInvalidRows(
        ImportSiteDataResult result,
        ImportExecutionOptions options)
    {
        if (options.SkipInvalidRows || result.SkippedCount == 0)
            return null;

        var firstError = result.Errors.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstError))
            firstError = "Import contains invalid rows and Import:SkipInvalidRows is disabled.";

        return Result.Failure<ImportSiteDataResult>(firstError);
    }

    private static bool LooksLikeExcel(byte[] fileContent)
    {
        // ZIP signatures (.xlsx/.xlsm)
        if (fileContent.Length >= 2 &&
            fileContent[0] == 0x50 &&
            fileContent[1] == 0x4B)
        {
            return true;
        }

        // Compound file signature (.xls)
        if (fileContent.Length >= 8 &&
            fileContent[0] == 0xD0 &&
            fileContent[1] == 0xCF &&
            fileContent[2] == 0x11 &&
            fileContent[3] == 0xE0 &&
            fileContent[4] == 0xA1 &&
            fileContent[5] == 0xB1 &&
            fileContent[6] == 0x1A &&
            fileContent[7] == 0xE1)
        {
            return true;
        }

        return false;
    }
}
