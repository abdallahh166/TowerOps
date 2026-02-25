namespace TowerOps.Application.Queries.Offices.GetOfficesByRegion;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.OfficeSpecifications;

public class GetOfficesByRegionQueryHandler : IRequestHandler<GetOfficesByRegionQuery, Result<List<OfficeDto>>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IMapper _mapper;

    public GetOfficesByRegionQueryHandler(
        IOfficeRepository officeRepository,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<OfficeDto>>> Handle(GetOfficesByRegionQuery request, CancellationToken cancellationToken)
    {
        var spec = new OfficesByRegionSpecification(request.Region, request.OnlyActive ?? true);
        var offices = await _officeRepository.FindAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<OfficeDto>>(offices.ToList());
        return Result.Success(dtos);
    }
}

