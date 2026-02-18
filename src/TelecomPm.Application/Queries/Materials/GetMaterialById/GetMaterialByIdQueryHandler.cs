namespace TelecomPM.Application.Queries.Materials.GetMaterialById;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Interfaces.Repositories;

public class GetMaterialByIdQueryHandler : IRequestHandler<GetMaterialByIdQuery, Result<MaterialDetailDto>>
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public GetMaterialByIdQueryHandler(
        IMaterialRepository materialRepository,
        IMapper mapper)
    {
        _materialRepository = materialRepository;
        _mapper = mapper;
    }

    public async Task<Result<MaterialDetailDto>> Handle(GetMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        var material = await _materialRepository.GetByIdAsNoTrackingAsync(request.MaterialId, cancellationToken);
        if (material == null)
            return Result.Failure<MaterialDetailDto>("Material not found");

        var dto = _mapper.Map<MaterialDetailDto>(material);
        return Result.Success(dto);
    }
}

