using MediatR;
using TelecomPM.Application.Commands.Signatures;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Signatures;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Signatures.GetWorkOrderSignatures;

public sealed class GetWorkOrderSignaturesQueryHandler : IRequestHandler<GetWorkOrderSignaturesQuery, Result<WorkOrderSignaturesDto>>
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetWorkOrderSignaturesQueryHandler(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Result<WorkOrderSignaturesDto>> Handle(GetWorkOrderSignaturesQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsNoTrackingAsync(request.WorkOrderId, cancellationToken);
        if (workOrder is null)
            return Result.Failure<WorkOrderSignaturesDto>("Work order not found.");

        return Result.Success(new WorkOrderSignaturesDto
        {
            ClientSignature = workOrder.ClientSignature is null ? null : SignatureMapper.ToDto(workOrder.ClientSignature),
            EngineerSignature = workOrder.EngineerSignature is null ? null : SignatureMapper.ToDto(workOrder.EngineerSignature)
        });
    }
}
