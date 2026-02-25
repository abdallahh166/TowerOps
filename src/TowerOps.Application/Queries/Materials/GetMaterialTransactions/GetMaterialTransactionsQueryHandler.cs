namespace TowerOps.Application.Queries.Materials.GetMaterialTransactions;

using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Domain.Interfaces.Repositories;

public class GetMaterialTransactionsQueryHandler : IRequestHandler<GetMaterialTransactionsQuery, Result<List<MaterialTransactionDto>>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public GetMaterialTransactionsQueryHandler(
        IMaterialRepository materialRepository,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MaterialTransactionDto>>> Handle(GetMaterialTransactionsQuery request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsNoTrackingAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure<List<MaterialTransactionDto>>("Material not found");

        var transactions = material.Transactions.AsQueryable();

        if (request.FromDate.HasValue)
        {
            transactions = transactions.Where(t => t.TransactionDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            transactions = transactions.Where(t => t.TransactionDate <= request.ToDate.Value);
        }

        var dtos = _mapper.Map<List<MaterialTransactionDto>>(transactions.OrderByDescending(t => t.TransactionDate).ToList());
        return Result.Success(dtos);
    }
}

