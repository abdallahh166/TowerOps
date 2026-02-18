namespace TelecomPM.Application.Commands.Offices.UpdateOfficeContact;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;
using TelecomPM.Domain.Interfaces.Repositories;

public class UpdateOfficeContactCommandHandler : IRequestHandler<UpdateOfficeContactCommand, Result<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateOfficeContactCommandHandler(
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OfficeDto>> Handle(UpdateOfficeContactCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<OfficeDto>("Office not found");

        try
        {
            office.SetContactInfo(request.ContactPerson, request.ContactPhone, request.ContactEmail);
            await _officeRepository.UpdateAsync(office, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<OfficeDto>(office);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<OfficeDto>($"Failed to update office contact: {ex.Message}");
        }
    }
}

