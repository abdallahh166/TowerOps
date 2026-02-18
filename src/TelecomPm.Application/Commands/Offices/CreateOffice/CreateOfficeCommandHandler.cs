namespace TelecomPM.Application.Commands.Offices.CreateOffice;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, Result<OfficeDto>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOfficeCommandHandler(
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<OfficeDto>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
    {
        // Check if office code already exists
        var existing = await _officeRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing != null)
            return Result.Failure<OfficeDto>($"Office with code {request.Code} already exists");

        try
        {
            var address = Address.Create(
                request.Street,
                request.City,
                request.Region,
                $"{request.BuildingNumber ?? ""}, {request.PostalCode ?? ""}".Trim(',', ' '));

            var office = Office.Create(
                request.Code,
                request.Name,
                request.Region,
                address);

            // Set coordinates if provided
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                var coordinates = Coordinates.Create(request.Latitude.Value, request.Longitude.Value);
                office.SetCoordinates(coordinates);
            }

            // Set contact info if provided
            if (!string.IsNullOrWhiteSpace(request.ContactPerson))
            {
                office.SetContactInfo(
                    request.ContactPerson,
                    request.ContactPhone ?? string.Empty,
                    request.ContactEmail ?? string.Empty);
            }

            await _officeRepository.AddAsync(office, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<OfficeDto>(office);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<OfficeDto>($"Failed to create office: {ex.Message}");
        }
    }
}

