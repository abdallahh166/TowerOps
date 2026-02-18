namespace TelecomPM.Application.Queries.Offices.GetAllOffices;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;
using TelecomPM.Domain.Interfaces.Repositories;

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
        var offices = request.OnlyActive ?? false
            ? await _officeRepository.GetAllAsNoTrackingAsync(cancellationToken)
            : await _officeRepository.GetAllAsNoTrackingAsync(cancellationToken);

        var filteredOffices = request.OnlyActive ?? false
            ? offices.Where(o => o.IsActive && !o.IsDeleted).ToList()
            : offices.Where(o => !o.IsDeleted).ToList();

        var dtos = _mapper.Map<List<OfficeDto>>(filteredOffices);
        return Result.Success(dtos);
    }
}

