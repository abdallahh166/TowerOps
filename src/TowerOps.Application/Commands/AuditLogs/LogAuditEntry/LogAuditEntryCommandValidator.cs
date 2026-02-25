namespace TowerOps.Application.Commands.AuditLogs.LogAuditEntry;

using FluentValidation;

public class LogAuditEntryCommandValidator : AbstractValidator<LogAuditEntryCommand>
{
    public LogAuditEntryCommandValidator()
    {
        RuleFor(x => x.EntityType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EntityId).NotEmpty();
        RuleFor(x => x.Action).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ActorId).NotEmpty();
        RuleFor(x => x.ActorRole).NotEmpty().MaximumLength(100);
    }
}
