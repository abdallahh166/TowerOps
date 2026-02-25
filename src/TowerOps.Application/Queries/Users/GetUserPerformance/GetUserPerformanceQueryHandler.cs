namespace TowerOps.Application.Queries.Users.GetUserPerformance;

using AutoMapper;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class GetUserPerformanceQueryHandler : IRequestHandler<GetUserPerformanceQuery, Result<UserPerformanceDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IMapper _mapper;

    public GetUserPerformanceQueryHandler(
        IUserRepository userRepository,
        IVisitRepository visitRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _visitRepository = visitRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserPerformanceDto>> Handle(GetUserPerformanceQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsNoTrackingAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserPerformanceDto>("User not found");

        if (user.Role != UserRole.PMEngineer)
            return Result.Failure<UserPerformanceDto>("Performance metrics are only available for PM Engineers");

        // Get visits for this engineer
        var allVisits = await _visitRepository.GetByEngineerIdAsNoTrackingAsync(request.UserId, cancellationToken);
        var visits = allVisits.AsEnumerable();

        // Apply date filter if provided
        if (request.FromDate.HasValue)
        {
            visits = visits.Where(v => v.ScheduledDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            visits = visits.Where(v => v.ScheduledDate <= request.ToDate.Value);
        }

        var visitsList = visits.ToList();

        // Calculate performance metrics
        var totalVisits = visitsList.Count;
        var completedVisits = visitsList.Count(v => v.Status == Domain.Enums.VisitStatus.Completed);
        var approvedVisits = visitsList.Count(v => v.Status == Domain.Enums.VisitStatus.Approved);
        var rejectedVisits = visitsList.Count(v => v.Status == Domain.Enums.VisitStatus.Rejected);
        var onTimeVisits = visitsList.Count(v => v.Status == Domain.Enums.VisitStatus.Completed && 
                                                   v.ActualEndTime.HasValue && 
                                                   v.ActualEndTime.Value.Date <= v.ScheduledDate.Date);

        var completionRate = totalVisits > 0 ? (decimal)completedVisits / totalVisits * 100 : 0;
        var approvalRate = completedVisits > 0 ? (decimal)approvedVisits / completedVisits * 100 : 0;
        var onTimeRate = completedVisits > 0 ? (decimal)onTimeVisits / completedVisits * 100 : 0;

        var performance = new UserPerformanceDto
        {
            UserId = user.Id,
            UserName = user.Name,
            UserEmail = user.Email,
            Role = user.Role,
            AssignedSitesCount = user.AssignedSiteIds.Count,
            MaxAssignedSites = user.MaxAssignedSites,
            TotalVisits = totalVisits,
            CompletedVisits = completedVisits,
            ApprovedVisits = approvedVisits,
            RejectedVisits = rejectedVisits,
            OnTimeVisits = onTimeVisits,
            CompletionRate = Math.Round(completionRate, 2),
            ApprovalRate = Math.Round(approvalRate, 2),
            OnTimeRate = Math.Round(onTimeRate, 2),
            PerformanceRating = user.PerformanceRating,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };

        return Result.Success(performance);
    }
}

