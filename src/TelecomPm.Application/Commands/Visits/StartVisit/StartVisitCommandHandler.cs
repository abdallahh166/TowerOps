namespace TelecomPM.Application.Commands.Visits.StartVisit;

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class StartVisitCommandHandler : IRequestHandler<StartVisitCommand, Result<VisitDto>>
{
    private static readonly HashSet<string> EquipmentOnlyExcludedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        ChecklistItemCategory.Tower.ToString(),
        ChecklistItemCategory.Generator.ToString(),
        ChecklistItemCategory.Fence.ToString(),
        ChecklistItemCategory.EarthBar.ToString()
    };

    private readonly IVisitRepository _visitRepository;
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<StartVisitCommandHandler> _logger;

    public StartVisitCommandHandler(
        IVisitRepository visitRepository,
        IChecklistTemplateRepository checklistTemplateRepository,
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<StartVisitCommandHandler> logger)
    {
        _visitRepository = visitRepository;
        _checklistTemplateRepository = checklistTemplateRepository;
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<VisitDto>> Handle(StartVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitDto>("Visit not found");

        try
        {
            var coordinates = Coordinates.Create(request.Latitude, request.Longitude);
            visit.StartVisit(coordinates);

            var templateVisitType = ResolveChecklistTemplateVisitType(visit.Type);
            var activeTemplate = await _checklistTemplateRepository.GetActiveByVisitTypeAsync(templateVisitType, cancellationToken);
            var site = await _siteRepository.GetByIdAsync(visit.SiteId, cancellationToken);
            var isEquipmentOnly = site?.ResponsibilityScope == ResponsibilityScope.EquipmentOnly;

            if (activeTemplate is not null)
            {
                foreach (var templateItem in activeTemplate.Items.OrderBy(i => i.OrderIndex))
                {
                    if (!IsTemplateItemApplicable(templateItem, visit.Type, site?.SiteType))
                    {
                        continue;
                    }

                    if (isEquipmentOnly &&
                        ShouldExcludeForEquipmentOnly(templateItem.Category, templateItem.ItemName))
                    {
                        continue;
                    }

                    visit.AddChecklistItem(
                        VisitChecklist.Create(
                            visit.Id,
                            templateItem.Category,
                            templateItem.ItemName,
                            templateItem.Description ?? string.Empty,
                            templateItem.IsMandatory,
                            templateItem.Id));
                }

                visit.ApplyChecklistTemplate(activeTemplate.Id, activeTemplate.Version);

                if (isEquipmentOnly)
                {
                    visit.AddChecklistItem(
                        VisitChecklist.Create(
                            visit.Id,
                            ChecklistItemCategory.General.ToString(),
                            "Report any tower/shared equipment issues to host operator",
                            "Escalate tower/shared infrastructure findings to the host operator.",
                            true));
                }
            }
            else
            {
                _logger.LogWarning(
                    "No active checklist template was found for visit {VisitId} and type {VisitType}",
                    visit.Id,
                    templateVisitType);
            }

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<VisitDto>(visit);
            return Result.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result.Failure<VisitDto>(ex.Message);
        }
    }

    private static VisitType ResolveChecklistTemplateVisitType(VisitType visitType)
    {
        return visitType.ToCanonical();
    }

    private static bool ShouldExcludeForEquipmentOnly(string category, string itemName)
    {
        var normalizedCategory = category?.Trim() ?? string.Empty;
        var normalizedItem = itemName?.Trim() ?? string.Empty;

        if (EquipmentOnlyExcludedCategories.Contains(normalizedCategory))
            return true;

        if (normalizedItem.Contains("tower", StringComparison.OrdinalIgnoreCase))
            return true;

        if (normalizedItem.Contains("generator", StringComparison.OrdinalIgnoreCase))
            return true;

        if (normalizedItem.Contains("fence", StringComparison.OrdinalIgnoreCase))
            return true;

        if (normalizedItem.Contains("earth bar", StringComparison.OrdinalIgnoreCase) ||
            normalizedItem.Contains("earthbar", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static bool IsTemplateItemApplicable(
        ChecklistTemplateItem templateItem,
        VisitType visitType,
        SiteType? siteType)
    {
        if (!IsVisitTypeApplicable(templateItem.ApplicableVisitTypes, visitType))
            return false;

        if (!IsSiteTypeApplicable(templateItem.ApplicableSiteTypes, siteType))
            return false;

        return true;
    }

    private static bool IsVisitTypeApplicable(string? applicableVisitTypesRaw, VisitType visitType)
    {
        var allowedVisitTypes = ParseApplicabilityList(applicableVisitTypesRaw);
        if (allowedVisitTypes.Count == 0 || allowedVisitTypes.Contains("ALL"))
            return true;

        var canonicalVisitType = visitType.ToCanonical();
        var aliases = GetVisitTypeAliases(canonicalVisitType);
        return aliases.Any(alias => allowedVisitTypes.Contains(alias));
    }

    private static bool IsSiteTypeApplicable(string? applicableSiteTypesRaw, SiteType? siteType)
    {
        var allowedSiteTypes = ParseApplicabilityList(applicableSiteTypesRaw);
        if (allowedSiteTypes.Count == 0 || allowedSiteTypes.Contains("ALL"))
            return true;

        if (!siteType.HasValue)
            return true;

        var aliases = GetSiteTypeAliases(siteType.Value);
        return aliases.Any(alias => allowedSiteTypes.Contains(alias));
    }

    private static HashSet<string> ParseApplicabilityList(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        IEnumerable<string> values;
        var trimmed = raw.Trim();

        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            try
            {
                values = JsonSerializer.Deserialize<string[]>(trimmed) ?? Array.Empty<string>();
            }
            catch (JsonException)
            {
                values = Array.Empty<string>();
            }
        }
        else
        {
            values = trimmed.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        return values
            .Select(NormalizeApplicabilityToken)
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static string NormalizeApplicabilityToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value
            .Trim()
            .ToUpperInvariant()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal);
    }

    private static IReadOnlyCollection<string> GetVisitTypeAliases(VisitType visitType)
    {
        var canonical = visitType.ToCanonical();
        return canonical switch
        {
            VisitType.BM => new[] { "BM", "PM", "PREVENTIVEMAINTENANCE", "INSPECTION" },
            VisitType.CM => new[] { "CM", "CORRECTIVEMAINTENANCE", "EMERGENCY" },
            VisitType.Audit => new[] { "AUDIT" },
            _ => new[] { NormalizeApplicabilityToken(canonical.ToString()) }
        };
    }

    private static IReadOnlyCollection<string> GetSiteTypeAliases(SiteType siteType)
    {
        return siteType switch
        {
            SiteType.GreenField => new[] { "GF", "GREENFIELD" },
            SiteType.RoofTop => new[] { "RT", "ROOFTOP" },
            SiteType.Indoor => new[] { "INDOOR" },
            SiteType.BTS => new[] { "BTS" },
            _ => new[] { NormalizeApplicabilityToken(siteType.ToString()) }
        };
    }
}
