namespace TowerOps.Application.Queries.Materials.GetLowStockMaterials;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Materials;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.MaterialSpecifications;

public class GetLowStockMaterialsQueryHandler : IRequestHandler<GetLowStockMaterialsQuery, Result<List<MaterialDto>>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public GetLowStockMaterialsQueryHandler(IMaterialRepository materialRepository, IMapper mapper)
    {
        _materialRepository = materialRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MaterialDto>>> Handle(GetLowStockMaterialsQuery request, CancellationToken cancellationToken)
    {
        var spec = new LowStockMaterialsSpecification(request.OfficeId);
        var materials = await _materialRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<MaterialDto>>(materials);
        return Result.Success(dtos);
    }
}