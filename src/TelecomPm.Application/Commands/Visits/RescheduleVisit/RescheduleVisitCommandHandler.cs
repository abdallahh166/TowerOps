namespace TelecomPM.Application.Commands.Visits.RescheduleVisit;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;
using AutoMapper;

public class RescheduleVisitCommandHandler : IRequestHandler<RescheduleVisitCommand, Result>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleVisitCommandHandler(
        IVisitRepository visitRepository,
        IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RescheduleVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit == null)
            return Result.Failure("Visit not found");

        if (visit.Status != Domain.Enums.VisitStatus.Scheduled)
            return Result.Failure("Only scheduled visits can be rescheduled");

        try
        {
            visit.Reschedule(request.NewScheduledDate, request.Reason);
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to reschedule visit: {ex.Message}");
        }
    }
}

