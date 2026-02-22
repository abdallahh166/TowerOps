using System.Text.Json;
using TelecomPM.Application.DTOs.Sync;
using TelecomPM.Domain.Entities.Sync;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Services;

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
                        var visit = await GetVisitAsync(payload.RootElement, cancellationToken);
                        if (visit is null)
                        {
                            item.MarkFailed("Visit not found.");
                            failed++;
                            break;
                        }

                        if (visit.Status is VisitStatus.Submitted or VisitStatus.UnderReview or VisitStatus.Approved or VisitStatus.Rejected)
                        {
                            item.MarkConflict("VisitAlreadySubmitted");
                            await AddConflictAsync(item.Id, "VisitAlreadySubmitted", "Rejected", cancellationToken);
                            conflicts++;
                        }
                        else
                        {
                            item.MarkProcessed();
                            processed++;
                        }

                        break;
                    }
                    case "AddPhoto":
                    {
                        var visit = await GetVisitAsync(payload.RootElement, cancellationToken);
                        if (visit is null)
                        {
                            item.MarkFailed("Visit not found.");
                            failed++;
                            break;
                        }

                        var photoHash = GetString(payload.RootElement, "photoHash");
                        if (!string.IsNullOrWhiteSpace(photoHash) &&
                            visit.Photos.Any(p =>
                                string.Equals(p.FileName, photoHash, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(p.FilePath, photoHash, StringComparison.OrdinalIgnoreCase)))
                        {
                            // Duplicate photo: skip and mark processed as idempotent behavior.
                            item.MarkProcessed();
                            processed++;
                        }
                        else
                        {
                            item.MarkProcessed();
                            processed++;
                        }

                        break;
                    }
                    case "AddReading":
                    {
                        var visit = await GetVisitAsync(payload.RootElement, cancellationToken);
                        if (visit is null)
                        {
                            item.MarkFailed("Visit not found.");
                            failed++;
                            break;
                        }

                        var visitIdRaw = GetString(payload.RootElement, "visitId");
                        var readingType = GetString(payload.RootElement, "readingType");
                        var readingKey = $"{visitIdRaw}:{readingType}";
                        var isDuplicate = !string.IsNullOrWhiteSpace(readingType) &&
                                          (visit.Readings.Any(r => string.Equals(r.ReadingType, readingType, StringComparison.OrdinalIgnoreCase)) ||
                                           seenReadingKeys.Contains(readingKey));

                        if (isDuplicate)
                        {
                            item.MarkConflict("StaleData");
                            await AddConflictAsync(item.Id, "StaleData", "ServerWins", cancellationToken);
                            conflicts++;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(readingType))
                            {
                                seenReadingKeys.Add(readingKey);
                            }

                            item.MarkProcessed();
                            processed++;
                        }

                        break;
                    }
                    case "CheckIn":
                    {
                        item.MarkProcessed();
                        processed++;
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
}
