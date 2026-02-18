namespace TelecomPM.Application.Queries.Offices.GetOfficeById;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;
using TelecomPM.Domain.Interfaces.Repositories;

public class GetOfficeByIdQueryHandler : IRequestHandler<GetOfficeByIdQuery, Result<OfficeDetailDto>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IMapper _mapper;

    public GetOfficeByIdQueryHandler(
        IOfficeRepository officeRepository,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _mapper = mapper;
    }

    public async Task<Result<OfficeDetailDto>> Handle(GetOfficeByIdQuery request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<OfficeDetailDto>("Office not found");

        var dto = _mapper.Map<OfficeDetailDto>(office);
        return Result.Success(dto);
    }
}

