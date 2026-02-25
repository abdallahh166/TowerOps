namespace TowerOps.Application.Commands.ApprovalRecords.CreateApprovalRecord;

using MediatR;
using TowerOps.Application.Common;
using TowerOps.Domain.Entities.ApprovalRecords;
using TowerOps.Domain.Interfaces.Repositories;

public class CreateApprovalRecordCommandHandler : IRequestHandler<CreateApprovalRecordCommand, Result<Guid>>
{
    private readonly IApprovalRecordRepository _approvalRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApprovalRecordCommandHandler(
        IApprovalRecordRepository approvalRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _approvalRecordRepository = approvalRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateApprovalRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var record = ApprovalRecord.Create(
                request.WorkOrderId,
                request.WorkflowType,
                request.StageName,
                request.ReviewerRole,
                request.ReviewerId,
                request.Decision,
                request.Reason);

            await _approvalRecordRepository.AddAsync(record, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(record.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>($"Failed to create approval record: {ex.Message}");
        }
    }
}
