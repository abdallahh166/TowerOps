namespace TowerOps.Application.Commands.Visits.AddPhoto;

using AutoMapper;
using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.ValueObjects;

public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, Result<VisitPhotoDto>>
{
    private readonly IEditableVisitMutationService _editableVisitMutationService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public AddPhotoCommandHandler(
        IEditableVisitMutationService editableVisitMutationService,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _editableVisitMutationService = editableVisitMutationService;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public Task<Result<VisitPhotoDto>> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        => _editableVisitMutationService.ExecuteAsync(
            request.VisitId,
            async visit =>
            {
                var containerName = $"visits/{visit.VisitNumber}/photos";
                var filePath = await _fileStorageService.UploadFileAsync(
                    request.FileStream,
                    request.FileName,
                    containerName,
                    cancellationToken);

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

                return _mapper.Map<VisitPhotoDto>(photo);
            },
            "Failed to add photo",
            cancellationToken);
}
