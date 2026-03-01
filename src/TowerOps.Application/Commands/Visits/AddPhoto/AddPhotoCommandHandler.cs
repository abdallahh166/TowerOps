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
    private readonly IUploadedFileValidationService _uploadedFileValidationService;
    private readonly ISystemSettingsService _settingsService;
    private readonly IMapper _mapper;

    public AddPhotoCommandHandler(
        IEditableVisitMutationService editableVisitMutationService,
        IFileStorageService fileStorageService,
        IUploadedFileValidationService uploadedFileValidationService,
        ISystemSettingsService settingsService,
        IMapper mapper)
    {
        _editableVisitMutationService = editableVisitMutationService;
        _fileStorageService = fileStorageService;
        _uploadedFileValidationService = uploadedFileValidationService;
        _settingsService = settingsService;
        _mapper = mapper;
    }

    public Task<Result<VisitPhotoDto>> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        => _editableVisitMutationService.ExecuteAsync(
            request.VisitId,
            async visit =>
            {
                var validation = await _uploadedFileValidationService.ValidateAsync(
                    request.FileName,
                    request.FileStream,
                    cancellationToken);

                if (!validation.IsValid)
                {
                    throw new InvalidOperationException(validation.Error ?? "Invalid upload file.");
                }

                var quarantineContainerRoot = await _settingsService.GetAsync(
                    "UploadSecurity:QuarantineContainer",
                    "quarantine",
                    cancellationToken);

                var containerName = $"{quarantineContainerRoot.TrimEnd('/')}/visits/{visit.VisitNumber}/photos";
                var originalFileName = Path.GetFileName(request.FileName);
                var generatedFileName = $"{Guid.NewGuid():N}{Path.GetExtension(originalFileName)}";

                var filePath = await _fileStorageService.UploadFileAsync(
                    request.FileStream,
                    generatedFileName,
                    containerName,
                    cancellationToken);

                var photo = VisitPhoto.CreatePendingUpload(
                    visit.Id,
                    request.Type,
                    request.Category,
                    request.ItemName,
                    originalFileName,
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
