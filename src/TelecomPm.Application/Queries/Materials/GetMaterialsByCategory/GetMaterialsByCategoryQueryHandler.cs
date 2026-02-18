namespace TelecomPM.Application.Queries.Materials.GetMaterialsByCategory;

using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.MaterialSpecifications;

public class GetMaterialsByCategoryQueryHandler : IRequestHandler<GetMaterialsByCategoryQuery, Result<List<MaterialDto>>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public GetMaterialsByCategoryQueryHandler(
        IMaterialRepository materialRepository,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MaterialDto>>> Handle(GetMaterialsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var spec = new MaterialsByCategorySpecification(request.Category, request.OfficeId);
        var materials = await _materialRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<MaterialDto>>(materials.ToList());
        return Result.Success(dtos);
    }
}

