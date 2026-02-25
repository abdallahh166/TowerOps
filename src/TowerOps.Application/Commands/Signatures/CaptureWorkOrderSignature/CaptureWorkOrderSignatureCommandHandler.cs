using MediatR;
using TowerOps.Application.Common;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Application.Commands.Signatures.CaptureWorkOrderSignature;

public sealed class CaptureWorkOrderSignatureCommandHandler : IRequestHandler<CaptureWorkOrderSignatureCommand, Result>
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CaptureWorkOrderSignatureCommandHandler(
        IWorkOrderRepository workOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _workOrderRepository = workOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CaptureWorkOrderSignatureCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId, cancellationToken);
        if (workOrder is null)
            return Result.Failure("Work order not found.");

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

            if (request.IsEngineerSignature)
            {
                workOrder.CaptureEngineerSignature(signature);
            }
            else
            {
                workOrder.CaptureClientSignature(signature);
            }

            await _workOrderRepository.UpdateAsync(workOrder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
