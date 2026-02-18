namespace TelecomPM.Application.Commands.Visits.AddPhoto;

using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, Result<VisitPhotoDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddPhotoCommandHandler(
        IVisitRepository visitRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitPhotoDto>> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure<VisitPhotoDto>("Visit not found");

        if (!visit.CanBeEdited())
            return Result.Failure<VisitPhotoDto>("Visit cannot be edited");

        try
        {
            // Upload photo to storage
            var containerName = $"visits/{visit.VisitNumber}/photos";
            var filePath = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                request.FileName,
                containerName,
                cancellationToken);

            // Create photo entity
            var photo = VisitPhoto.Create(
                visit.Id,
                request.Type,
                request.Category,
                request.ItemName,
                request.FileName,
                filePath);

            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                var coords = Coordinates.Create(request.Latitude.Value, request.Longitude.Value);
                photo.SetLocation(coords);
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                photo.SetDescription(request.Description);
            }

            visit.AddPhoto(photo);

            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<VisitPhotoDto>(photo);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<VisitPhotoDto>($"Failed to add photo: {ex.Message}");
        }
    }
}