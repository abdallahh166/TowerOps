namespace TelecomPM.Application.Queries.Materials.GetMaterialUsageReport;

using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;
using TelecomPM.Domain.Interfaces.Repositories;

public class GetMaterialUsageReportQueryHandler : IRequestHandler<GetMaterialUsageReportQuery, Result<MaterialUsageReportDto>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetMaterialUsageReportQueryHandler(
        IMaterialRepository materialRepository,
        IVisitRepository visitRepository,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<MaterialUsageReportDto>> Handle(GetMaterialUsageReportQuery request, CancellationToken cancellationToken)
    {
        // Get materials
        var materials = new List<Domain.Entities.Materials.Material>();
        if (request.MaterialId.HasValue)
        {
            var material = await _materialRepository.GetByIdAsNoTrackingAsync(request.MaterialId.Value, cancellationToken);
            if (material is not null)
            {
                materials.Add(material);
            }
        }
        else
        {
            materials = (await _materialRepository.GetByOfficeIdAsNoTrackingAsync(request.OfficeId ?? Guid.Empty, cancellationToken)).ToList();
        }

        materials = materials
            .Where(m => !request.OfficeId.HasValue || m.OfficeId == request.OfficeId.Value)
            .ToList();

        // Get transactions in date range
        var allTransactions = materials
            .SelectMany(m => m.Transactions
                .Where(t => t.TransactionDate >= request.FromDate && t.TransactionDate <= request.ToDate))
            .ToList();

        // Calculate usage summary
        var usageItems = materials.Select(m => new MaterialUsageItemDto
        {
            MaterialId = m.Id,
            MaterialCode = m.Code,
            MaterialName = m.Name,
            TotalUsed = m.Transactions
                .Where(t => t.Type == Domain.Entities.Materials.TransactionType.Usage && 
                           t.TransactionDate >= request.FromDate && 
                           t.TransactionDate <= request.ToDate)
                .Sum(t => t.Quantity.Value),
            TotalPurchased = m.Transactions
                .Where(t => t.Type == Domain.Entities.Materials.TransactionType.Purchase && 
                           t.TransactionDate >= request.FromDate && 
                           t.TransactionDate <= request.ToDate)
                .Sum(t => t.Quantity.Value),
            Unit = m.CurrentStock.Unit.ToString(),
            TotalCost = m.Transactions
                .Where(t => t.Type == Domain.Entities.Materials.TransactionType.Usage && 
                           t.TransactionDate >= request.FromDate && 
                           t.TransactionDate <= request.ToDate)
                .Sum(t => t.Quantity.Value * m.UnitCost.Amount)
        }).ToList();

        var report = new MaterialUsageReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            OfficeId = request.OfficeId,
            Items = usageItems,
            TotalMaterialsTracked = materials.Count,
            TotalTransactions = allTransactions.Count
        };

        return Result.Success(report);
    }
}
