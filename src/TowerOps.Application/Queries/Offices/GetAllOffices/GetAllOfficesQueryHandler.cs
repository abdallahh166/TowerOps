namespace TowerOps.Application.Queries.Offices.GetAllOffices;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.OfficeSpecifications;

public class GetAllOfficesQueryHandler : IRequestHandler<GetAllOfficesQuery, Result<PaginatedList<OfficeDto>>>
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

    public async Task<Result<PaginatedList<OfficeDto>>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var onlyActive = request.OnlyActive ?? false;
        var pageNumber = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "code" : request.SortBy.Trim();

        var specification = new AllOfficesFilteredSpecification(
            onlyActive,
            (pageNumber - 1) * pageSize,
            pageSize,
            sortBy,
            request.SortDescending);

        var totalCount = await _officeRepository.CountAsync(specification, cancellationToken);
        var filteredOffices = await _officeRepository.FindAsNoTrackingAsync(specification, cancellationToken);
        var dtos = _mapper.Map<List<OfficeDto>>(filteredOffices);
        var paged = new PaginatedList<OfficeDto>(dtos, totalCount, pageNumber, pageSize);
        return Result.Success(paged);
    }
}

