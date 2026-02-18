namespace TelecomPM.Application.Commands.Offices.UpdateOffice;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class UpdateOfficeCommandHandler : IRequestHandler<UpdateOfficeCommand, Result<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OfficeDto>> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<OfficeDto>("Office not found");

        try
        {
            var address = Address.Create(
                request.Street,
                request.City,
                request.Region,
                $"{request.BuildingNumber ?? ""}, {request.PostalCode ?? ""}".Trim(',', ' '));

            office.UpdateInfo(request.Name, request.Region, address);
            await _officeRepository.UpdateAsync(office, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<OfficeDto>(office);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<OfficeDto>($"Failed to update office: {ex.Message}");
        }
    }
}

