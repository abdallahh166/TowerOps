using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Application.Commands.Signatures.CaptureVisitSignature;

public sealed class CaptureVisitSignatureCommandHandler : IRequestHandler<CaptureVisitSignatureCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CaptureVisitSignatureCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CaptureVisitSignatureCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure("Visit not found.");

        try
        {
            var location = (request.Latitude.HasValue && request.Longitude.HasValue)
                ? GeoLocation.Create(request.Latitude.Value, request.Longitude.Value)
                : null;

            var signature = Signature.Create(
                request.SignerName,
                request.SignerRole,
                request.SignatureDataBase64,
                request.SignerPhone,
                location);

            visit.CaptureSiteContactSignature(signature);
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
