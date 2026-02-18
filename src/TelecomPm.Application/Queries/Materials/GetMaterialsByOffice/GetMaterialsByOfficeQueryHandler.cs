namespace TelecomPM.Application.Queries.Materials.GetMaterialsByOffice;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications.MaterialSpecifications;

public class GetMaterialsByOfficeQueryHandler : IRequestHandler<GetMaterialsByOfficeQuery, Result<List<MaterialDto>>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public GetMaterialsByOfficeQueryHandler(
        IMaterialRepository materialRepository,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MaterialDto>>> Handle(GetMaterialsByOfficeQuery request, CancellationToken cancellationToken)
    {
        var spec = new MaterialsByOfficeSpecification(request.OfficeId, request.OnlyInStock ?? false);
        var materials = await _materialRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<MaterialDto>>(materials.ToList());
        return Result.Success(dtos);
    }
}

