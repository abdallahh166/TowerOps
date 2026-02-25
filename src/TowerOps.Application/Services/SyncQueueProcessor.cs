using System.Text.Json;
using TowerOps.Application.DTOs.Sync;
using TowerOps.Domain.Entities.Sync;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using TowerOps.Domain.Entities.Visits;
using System.Globalization;

namespace TowerOps.Application.Services;

public sealed class SyncQueueProcessor : ISyncQueueProcessor
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISyncConflictRepository _syncConflictRepository;

    public SyncQueueProcessor(
        IVisitRepository visitRepository,
        ISyncConflictRepository syncConflictRepository)
    {
        _visitRepository = visitRepository;
        _syncConflictRepository = syncConflictRepository;
    }

    public async Task<SyncResultDto> ProcessAsync(IReadOnlyList<SyncQueue> queuedItems, CancellationToken cancellationToken = default)
    {
        var processed = 0;
        var conflicts = 0;
        var failed = 0;
        var seenReadingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in queuedItems.OrderBy(x => x.CreatedOnDeviceUtc))
        {
            try
            {
                using var payload = JsonDocument.Parse(string.IsNullOrWhiteSpace(item.Payload) ? "{}" : item.Payload);

                switch (item.OperationType.Trim())
                {
                    case "SubmitChecklist":
                    {
                        var outcome = await HandleSubmitChecklistAsync(item, payload.RootElement, cancellationToken);
                        Accumulate(outcome, ref processed, ref conflicts, ref failed);
                        break;
                    }
                    case "AddPhoto":
                    {
                        var outcome = await HandleAddPhotoAsync(item, payload.RootElement, cancellationToken);
                        Accumulate(outcome, ref processed, ref conflicts, ref failed);
                        break;
                    }
                    case "AddReading":
                    {
                        var outcome = await HandleAddReadingAsync(item, payload.RootElement, seenReadingKeys, cancellationToken);
                        Accumulate(outcome, ref processed, ref conflicts, ref failed);
                        break;
                    }
                    case "CheckIn":
                    {
                        var outcome = await HandleCheckInAsync(item, payload.RootElement, cancellationToken);
                        Accumulate(outcome, ref processed, ref conflicts, ref failed);
                        break;
                    }
                    default:
                    {
                        item.MarkFailed($"Unsupported operation '{item.OperationType}'.");
                        failed++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                item.MarkFailed(ex.Message);
                failed++;
            }
        }

        return new SyncResultDto
        {
            Processed = processed,
            Conflicts = conflicts,
            Failed = failed,
            Skipped = 0
        };
    }

    private async Task<ProcessingOutcome> HandleSubmitChecklistAsync(
        SyncQueue item,
        JsonElement payload,
        CancellationToken cancellationToken)
    {
        var visit = await GetVisitAsync(payload, cancellationToken);
        if (visit is null)
        {
            item.MarkFailed("Visit not found.");
            return ProcessingOutcome.Failed;
        }

        if (visit.Status is VisitStatus.Submitted or VisitStatus.UnderReview or VisitStatus.Approved or VisitStatus.Rejected)
        {
            item.MarkConflict("VisitAlreadySubmitted");
            await AddConflictAsync(item.Id, "VisitAlreadySubmitted", "Rejected", cancellationToken);
            return ProcessingOutcome.Conflict;
        }

        try
        {
            visit.Submit();
            item.MarkProcessed();
            return ProcessingOutcome.Processed;
        }
        catch (DomainException ex)
        {
            item.MarkFailed(ex.Message);
            return ProcessingOutcome.Failed;
        }
    }

    private async Task<ProcessingOutcome> HandleAddPhotoAsync(
        SyncQueue item,
        JsonElement payload,
        CancellationToken cancellationToken)
    {
        var visit = await GetVisitAsync(payload, cancellationToken);
        if (visit is null)
        {
            item.MarkFailed("Visit not found.");
            return ProcessingOutcome.Failed;
        }

        var photoHash = GetString(payload, "photoHash");
        var fileName = GetString(payload, "fileName");
        var filePath = GetString(payload, "filePath");
        var dedupeToken = photoHash ?? fileName ?? filePath ?? string.Empty;

        var isDuplicate = !string.IsNullOrWhiteSpace(dedupeToken) &&
                          visit.Photos.Any(p =>
                              string.Equals(p.FileName, dedupeToken, StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(p.FilePath, dedupeToken, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate)
        {
            // Duplicate photo is idempotent and treated as processed.
            item.MarkProcessed();
            return ProcessingOutcome.Processed;
        }

        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(filePath))
        {
            item.MarkFailed("Photo payload must include fileName and filePath.");
            return ProcessingOutcome.Failed;
        }

        try
        {
            var type = ParseEnum(payload, "type", PhotoType.During);
            var category = ParseEnum(payload, "category", PhotoCategory.Other);
            var itemName = GetString(payload, "itemName") ?? "Offline Sync Photo";

            var photo = VisitPhoto.Create(
                visit.Id,
                type,
                category,
                itemName,
                fileName,
                filePath);

            var description = GetString(payload, "description");
            if (!string.IsNullOrWhiteSpace(description))
                photo.SetDescription(description);

            var lat = GetDecimal(payload, "latitude");
            var lng = GetDecimal(payload, "longitude");
            if (lat.HasValue && lng.HasValue)
            {
                photo.SetLocation(Coordinates.Create((double)lat.Value, (double)lng.Value));
            }

            visit.AddPhoto(photo);
            item.MarkProcessed();
            return ProcessingOutcome.Processed;
        }
        catch (DomainException ex)
        {
            item.MarkFailed(ex.Message);
            return ProcessingOutcome.Failed;
        }
    }

    private async Task<ProcessingOutcome> HandleAddReadingAsync(
        SyncQueue item,
        JsonElement payload,
        ISet<string> seenReadingKeys,
        CancellationToken cancellationToken)
    {
        var visit = await GetVisitAsync(payload, cancellationToken);
        if (visit is null)
        {
            item.MarkFailed("Visit not found.");
            return ProcessingOutcome.Failed;
        }

        var visitIdRaw = GetString(payload, "visitId");
        var readingType = GetString(payload, "readingType");
        if (string.IsNullOrWhiteSpace(readingType))
        {
            item.MarkFailed("Reading payload must include readingType.");
            return ProcessingOutcome.Failed;
        }

        var readingKey = $"{visitIdRaw}:{readingType}";
        var isDuplicate = visit.Readings.Any(r => string.Equals(r.ReadingType, readingType, StringComparison.OrdinalIgnoreCase)) ||
                          seenReadingKeys.Contains(readingKey);

        if (isDuplicate)
        {
            item.MarkConflict("StaleData");
            await AddConflictAsync(item.Id, "StaleData", "ServerWins", cancellationToken);
            return ProcessingOutcome.Conflict;
        }

        var value = GetDecimal(payload, "value");
        if (!value.HasValue)
        {
            item.MarkFailed("Reading payload must include numeric value.");
            return ProcessingOutcome.Failed;
        }

        try
        {
            var category = GetString(payload, "category") ?? "General";
            var unit = GetString(payload, "unit") ?? "N/A";

            var reading = VisitReading.Create(visit.Id, readingType, category, value.Value, unit);

            var min = GetDecimal(payload, "minAcceptable");
            var max = GetDecimal(payload, "maxAcceptable");
            if (min.HasValue && max.HasValue)
                reading.SetValidationRange(min.Value, max.Value);

            var phase = GetString(payload, "phase");
            if (!string.IsNullOrWhiteSpace(phase))
                reading.SetPhase(phase);

            var equipment = GetString(payload, "equipment");
            if (!string.IsNullOrWhiteSpace(equipment))
                reading.SetEquipment(equipment);

            var notes = GetString(payload, "notes");
            if (!string.IsNullOrWhiteSpace(notes))
                reading.AddNotes(notes);

            visit.AddReading(reading);
            seenReadingKeys.Add(readingKey);

            item.MarkProcessed();
            return ProcessingOutcome.Processed;
        }
        catch (DomainException ex)
        {
            item.MarkFailed(ex.Message);
            return ProcessingOutcome.Failed;
        }
    }

    private async Task<ProcessingOutcome> HandleCheckInAsync(
        SyncQueue item,
        JsonElement payload,
        CancellationToken cancellationToken)
    {
        var visit = await GetVisitAsync(payload, cancellationToken);
        if (visit is null)
        {
            item.MarkFailed("Visit not found.");
            return ProcessingOutcome.Failed;
        }

        var latitude = GetDecimal(payload, "latitude");
        var longitude = GetDecimal(payload, "longitude");
        if (!latitude.HasValue || !longitude.HasValue)
        {
            item.MarkFailed("Check-in payload must include latitude and longitude.");
            return ProcessingOutcome.Failed;
        }

        var engineerLocation = GeoLocation.Create(latitude.Value, longitude.Value);

        decimal distanceFromSiteMeters;
        var explicitDistance = GetDecimal(payload, "distanceFromSiteMeters");
        if (explicitDistance.HasValue)
        {
            distanceFromSiteMeters = explicitDistance.Value;
        }
        else
        {
            var siteLatitude = GetDecimal(payload, "siteLatitude");
            var siteLongitude = GetDecimal(payload, "siteLongitude");
            if (siteLatitude.HasValue && siteLongitude.HasValue)
            {
                var siteLocation = GeoLocation.Create(siteLatitude.Value, siteLongitude.Value);
                distanceFromSiteMeters = engineerLocation.DistanceTo(siteLocation);
            }
            else
            {
                distanceFromSiteMeters = 0;
            }
        }

        var allowedRadius = GetDecimal(payload, "allowedRadiusMeters") ?? 200m;
        var explicitWithinRadius = GetBoolean(payload, "isWithinSiteRadius");
        var isWithinRadius = explicitWithinRadius ?? distanceFromSiteMeters <= allowedRadius;

        try
        {
            visit.RecordCheckIn(engineerLocation, distanceFromSiteMeters, isWithinRadius);
            item.MarkProcessed();
            return ProcessingOutcome.Processed;
        }
        catch (DomainException ex)
        {
            item.MarkFailed(ex.Message);
            return ProcessingOutcome.Failed;
        }
    }

    private async Task AddConflictAsync(
        Guid syncQueueId,
        string conflictType,
        string resolution,
        CancellationToken cancellationToken)
    {
        var conflict = SyncConflict.Create(syncQueueId, conflictType, resolution);
        await _syncConflictRepository.AddAsync(conflict, cancellationToken);
    }

    private async Task<Domain.Entities.Visits.Visit?> GetVisitAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var visitIdRaw = GetString(payload, "visitId");
        if (!Guid.TryParse(visitIdRaw, out var visitId))
            return null;

        return await _visitRepository.GetByIdAsync(visitId, cancellationToken);
    }

    private static string? GetString(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var element))
            return null;

        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };
    }

    private static decimal? GetDecimal(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var element))
            return null;

        return element.ValueKind switch
        {
            JsonValueKind.Number when element.TryGetDecimal(out var decimalValue) => decimalValue,
            JsonValueKind.String when decimal.TryParse(
                element.GetString(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var parsed) => parsed,
            _ => null
        };
    }

    private static bool? GetBoolean(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out var element))
            return null;

        return element.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number when element.TryGetInt32(out var intValue) => intValue != 0,
            JsonValueKind.String when bool.TryParse(element.GetString(), out var boolValue) => boolValue,
            JsonValueKind.String when element.GetString() == "1" => true,
            JsonValueKind.String when element.GetString() == "0" => false,
            _ => null
        };
    }

    private static TEnum ParseEnum<TEnum>(JsonElement payload, string propertyName, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        if (!payload.TryGetProperty(propertyName, out var element))
            return defaultValue;

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var intValue))
        {
            if (Enum.IsDefined(typeof(TEnum), intValue))
                return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            var raw = element.GetString();
            if (!string.IsNullOrWhiteSpace(raw) && Enum.TryParse<TEnum>(raw, true, out var parsed))
                return parsed;
        }

        return defaultValue;
    }

    private static void Accumulate(
        ProcessingOutcome outcome,
        ref int processed,
        ref int conflicts,
        ref int failed)
    {
        switch (outcome)
        {
            case ProcessingOutcome.Processed:
                processed++;
                break;
            case ProcessingOutcome.Conflict:
                conflicts++;
                break;
            default:
                failed++;
                break;
        }
    }

    private enum ProcessingOutcome
    {
        Processed,
        Conflict,
        Failed
    }
}
