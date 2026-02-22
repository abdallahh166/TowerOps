using MediatR;
using Microsoft.Extensions.Logging;
using TelecomPM.Application.Commands.ApprovalRecords.CreateApprovalRecord;
using TelecomPM.Application.Common.Events;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events.VisitEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.VisitEventHandlers;

public class VisitApprovedApprovalRecordEventHandler : INotificationHandler<DomainEventNotification<VisitApprovedEvent>>
{
    private readonly ISender _sender;
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitApprovedApprovalRecordEventHandler> _logger;

    public VisitApprovedApprovalRecordEventHandler(
        ISender sender,
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        ILogger<VisitApprovedApprovalRecordEventHandler> logger)
    {
        _sender = sender;
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitApprovedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var visit = await _visitRepository.GetByIdAsync(domainEvent.VisitId, cancellationToken);
            var reviewer = await _userRepository.GetByIdAsync(domainEvent.ReviewerId, cancellationToken);
            if (visit is null || reviewer is null)
                return;

            await _sender.Send(new CreateApprovalRecordCommand
            {
                WorkOrderId = domainEvent.VisitId,
                WorkflowType = MapWorkflow(visit.Type),
                StageName = "VisitReview",
                ReviewerRole = reviewer.Role.ToString(),
                ReviewerId = domainEvent.ReviewerId,
                Decision = ApprovalDecision.Approved
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create approval record for approved visit {VisitId}", domainEvent.VisitId);
        }
    }

    private static WorkflowType MapWorkflow(VisitType visitType)
        => visitType.IsCm() ? WorkflowType.CM : WorkflowType.BM;
}

public class VisitRejectedApprovalRecordEventHandler : INotificationHandler<DomainEventNotification<VisitRejectedEvent>>
{
    private readonly ISender _sender;
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitRejectedApprovalRecordEventHandler> _logger;

    public VisitRejectedApprovalRecordEventHandler(
        ISender sender,
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        ILogger<VisitRejectedApprovalRecordEventHandler> logger)
    {
        _sender = sender;
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitRejectedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var visit = await _visitRepository.GetByIdAsync(domainEvent.VisitId, cancellationToken);
            var reviewer = await _userRepository.GetByIdAsync(domainEvent.ReviewerId, cancellationToken);
            if (visit is null || reviewer is null)
                return;

            await _sender.Send(new CreateApprovalRecordCommand
            {
                WorkOrderId = domainEvent.VisitId,
                WorkflowType = MapWorkflow(visit.Type),
                StageName = "VisitReview",
                ReviewerRole = reviewer.Role.ToString(),
                ReviewerId = domainEvent.ReviewerId,
                Decision = ApprovalDecision.Rejected,
                Reason = domainEvent.Reason
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create approval record for rejected visit {VisitId}", domainEvent.VisitId);
        }
    }

    private static WorkflowType MapWorkflow(VisitType visitType)
        => visitType.IsCm() ? WorkflowType.CM : WorkflowType.BM;
}

public class VisitCorrectionRequestedApprovalRecordEventHandler : INotificationHandler<DomainEventNotification<VisitCorrectionRequestedEvent>>
{
    private readonly ISender _sender;
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitCorrectionRequestedApprovalRecordEventHandler> _logger;

    public VisitCorrectionRequestedApprovalRecordEventHandler(
        ISender sender,
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        ILogger<VisitCorrectionRequestedApprovalRecordEventHandler> logger)
    {
        _sender = sender;
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitCorrectionRequestedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var visit = await _visitRepository.GetByIdAsync(domainEvent.VisitId, cancellationToken);
            var reviewer = await _userRepository.GetByIdAsync(domainEvent.ReviewerId, cancellationToken);
            if (visit is null || reviewer is null)
                return;

            await _sender.Send(new CreateApprovalRecordCommand
            {
                WorkOrderId = domainEvent.VisitId,
                WorkflowType = MapWorkflow(visit.Type),
                StageName = "VisitReview",
                ReviewerRole = reviewer.Role.ToString(),
                ReviewerId = domainEvent.ReviewerId,
                Decision = ApprovalDecision.ReworkRequested,
                Reason = domainEvent.CorrectionNotes
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create approval record for correction-requested visit {VisitId}", domainEvent.VisitId);
        }
    }

    private static WorkflowType MapWorkflow(VisitType visitType)
        => visitType.IsCm() ? WorkflowType.CM : WorkflowType.BM;
}
