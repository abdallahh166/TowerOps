namespace TowerOps.Application.Commands.AuditLogs.LogAuditEntry;

using MediatR;
using TowerOps.Application.Common;
using TowerOps.Domain.Entities.AuditLogs;
using TowerOps.Domain.Interfaces.Repositories;

public class LogAuditEntryCommandHandler : IRequestHandler<LogAuditEntryCommand, Result<Guid>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogAuditEntryCommandHandler(
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(LogAuditEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entry = AuditLog.Create(
                request.EntityType,
                request.EntityId,
                request.Action,
                request.ActorId,
                request.ActorRole,
                request.PreviousState,
                request.NewState,
                request.Notes);

            await _auditLogRepository.AddAsync(entry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(entry.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>($"Failed to log audit entry: {ex.Message}");
        }
    }
}
