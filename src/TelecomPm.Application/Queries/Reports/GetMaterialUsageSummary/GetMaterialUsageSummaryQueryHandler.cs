namespace TelecomPM.Application.Queries.Reports.GetMaterialUsageSummary;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.VisitSpecifications;

public class GetMaterialUsageSummaryQueryHandler 
    : IRequestHandler<GetMaterialUsageSummaryQuery, Result<MaterialUsageSummaryDto>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly ISiteRepository _siteRepository;

    public GetMaterialUsageSummaryQueryHandler(
        IMaterialRepository materialRepository,
        IVisitRepository visitRepository,
        IOfficeRepository officeRepository,
        ISiteRepository siteRepository)
    {
        _materialRepository = materialRepository;
        _visitRepository = visitRepository;
        _officeRepository = officeRepository;
        _siteRepository = siteRepository;
    }

    public async Task<Result<MaterialUsageSummaryDto>> Handle(
        GetMaterialUsageSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsNoTrackingAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure<MaterialUsageSummaryDto>("Material not found");

        var office = await _officeRepository.GetByIdAsNoTrackingAsync(material.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<MaterialUsageSummaryDto>("Office not found");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-3);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        // Get all visits in date range
        var visitSpec = new VisitsByDateRangeSpecification(fromDate, toDate);
        var visits = await _visitRepository.FindAsNoTrackingAsync(visitSpec, cancellationToken);

        // Get material transactions
        var transactions = material.Transactions
            .Where(t => t.CreatedAt >= fromDate && t.CreatedAt <= toDate)
            .ToList();

        // Calculate usage statistics
        var consumedTransactions = transactions
            .Where(t => t.Type == Domain.Entities.Materials.TransactionType.Usage)
            .ToList();
        var purchasedTransactions = transactions
            .Where(t => t.Type == Domain.Entities.Materials.TransactionType.Purchase)
            .ToList();
        var transferredTransactions = transactions
            .Where(t => t.Type == Domain.Entities.Materials.TransactionType.Transfer)
            .ToList();

        var totalConsumed = consumedTransactions.Sum(t => t.Quantity.Value);
        var totalPurchased = purchasedTransactions.Sum(t => t.Quantity.Value);
        var totalTransferred = transferredTransactions.Sum(t => t.Quantity.Value);

        // Material usage in visits
        var materialUsages = visits
            .SelectMany(v => v.MaterialsUsed)
            .Where(m => m.MaterialCode == material.Code)
            .ToList();

        var visitUsageCount = materialUsages.Select(m => m.VisitId).Distinct().Count();
        var totalCost = materialUsages.Sum(m => m.TotalCost.Amount);
        var averageCostPerVisit = visitUsageCount > 0 ? totalCost / visitUsageCount : 0;

        // Usage trends (monthly)
        var usageTrends = materialUsages
            .GroupBy(m => new { m.CreatedAt.Year, m.CreatedAt.Month })
            .Select(g => new MaterialUsageTrendDto
            {
                Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                Consumed = g.Sum(m => m.Quantity.Value),
                Purchased = purchasedTransactions
                    .Where(t => t.CreatedAt.Year == g.Key.Year && t.CreatedAt.Month == g.Key.Month)
                    .Sum(t => t.Quantity.Value),
                AveragePerVisit = g.Select(m => m.VisitId).Distinct().Count() > 0
                    ? g.Sum(m => m.Quantity.Value) / g.Select(m => m.VisitId).Distinct().Count()
                    : 0
            })
            .OrderBy(t => t.Period)
            .ToList();

        // Top usage sites
        var siteUsages = materialUsages
            .GroupBy(m => m.VisitId)
            .Select(g => new { VisitId = g.Key, TotalCost = g.Sum(m => m.TotalCost.Amount), Quantity = g.Sum(m => m.Quantity.Value) })
            .ToList();

        var topUsageSites = new List<MaterialUsageBySiteDto>();
        foreach (var usage in siteUsages.Take(10))
        {
            var visit = visits.FirstOrDefault(v => v.Id == usage.VisitId);
            if (visit != null)
            {
                var site = await _siteRepository.GetByIdAsNoTrackingAsync(visit.SiteId, cancellationToken);
                if (site != null)
                {
                    topUsageSites.Add(new MaterialUsageBySiteDto
                    {
                        SiteId = site.Id,
                        SiteCode = site.SiteCode.Value,
                        SiteName = site.Name,
                        QuantityUsed = usage.Quantity,
                        TotalCost = usage.TotalCost,
                        VisitCount = siteUsages.Count(u => u.VisitId == usage.VisitId)
                    });
                }
            }
        }

        var daysSinceLastRestock = material.LastRestockDate.HasValue
            ? (int)(DateTime.UtcNow - material.LastRestockDate.Value).TotalDays
            : 0;

        var report = new MaterialUsageSummaryDto
        {
            MaterialId = material.Id,
            MaterialCode = material.Code,
            MaterialName = material.Name,
            Category = material.Category,
            OfficeId = office.Id,
            OfficeName = office.Name,
            CurrentStock = material.CurrentStock.Value,
            Unit = material.CurrentStock.Unit.ToString(),
            MinimumStock = material.MinimumStock.Value,
            ReorderQuantity = material.ReorderQuantity?.Value ?? 0,
            IsLowStock = material.IsStockLow(),
            StockValue = material.CurrentStock.Value * material.UnitCost.Amount,
            TotalConsumed = totalConsumed,
            TotalPurchased = totalPurchased,
            TotalTransferred = totalTransferred,
            TransactionCount = transactions.Count,
            VisitUsageCount = visitUsageCount,
            UnitCost = material.UnitCost.Amount,
            Currency = material.UnitCost.Currency,
            TotalCost = totalCost,
            AverageCostPerVisit = averageCostPerVisit,
            Supplier = material.Supplier,
            LastRestockDate = material.LastRestockDate,
            DaysSinceLastRestock = daysSinceLastRestock,
            UsageTrends = usageTrends,
            TopUsageSites = topUsageSites.OrderByDescending(s => s.TotalCost).Take(10).ToList(),
            FromDate = fromDate,
            ToDate = toDate
        };

        return Result.Success(report);
    }
}

