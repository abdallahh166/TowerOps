namespace TowerOps.Application.Queries.Offices.GetAllOffices;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.OfficeSpecifications;

public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, Result<List<OfficeDto>>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IMapper _mapper;

    public GetAllOfficesQueryHandler(
        IOfficeRepository officeRepository,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<OfficeDto>>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var onlyActive = request.OnlyActive ?? false;
        var pageNumber = request.PageNumber.GetValueOrDefault(1);
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        var pageSize = request.PageSize.GetValueOrDefault(100);
        if (pageSize < 1)
        {
            pageSize = 1;
        }

        if (pageSize > 200)
        {
            pageSize = 200;
        }

        var specification = new AllOfficesFilteredSpecification(
            onlyActive,
            (pageNumber - 1) * pageSize,
            pageSize);
        var filteredOffices = await _officeRepository.FindAsNoTrackingAsync(specification, cancellationToken);

        var dtos = _mapper.Map<List<OfficeDto>>(filteredOffices);
        return Result.Success(dtos);
    }
}

