namespace TowerOps.Application.Commands.ApprovalRecords.CreateApprovalRecord;

using FluentValidation;

public class CreateApprovalRecordCommandValidator : AbstractValidator<CreateApprovalRecordCommand>
{
    public CreateApprovalRecordCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty();
        RuleFor(x => x.StageName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReviewerRole).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReviewerId).NotEmpty();
    }
}
