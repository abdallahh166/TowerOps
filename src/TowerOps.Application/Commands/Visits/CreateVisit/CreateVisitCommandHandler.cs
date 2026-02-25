namespace TowerOps.Application.Commands.Visits.CreateVisit;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Services;

public class CreateVisitCommandHandler : IRequestHandler<CreateVisitCommand, Result<VisitDto>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVisitNumberGeneratorService _visitNumberGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateVisitCommandHandler(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        IVisitNumberGeneratorService visitNumberGenerator,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _visitNumberGenerator = visitNumberGenerator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VisitDto>> Handle(CreateVisitCommand request, CancellationToken cancellationToken)
    {
        // Validate site exists
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure<VisitDto>("Site not found");

        if (!site.CanBeVisited())
            return Result.Failure<VisitDto>("Site cannot be visited (off-air or deleted)");

        // Validate engineer exists
        var engineer = await _userRepository.GetByIdAsync(request.EngineerId, cancellationToken);
        if (engineer == null)
            return Result.Failure<VisitDto>("Engineer not found");

        if (engineer.Role != UserRole.PMEngineer)
            return Result.Failure<VisitDto>("User is not a PM Engineer");

        if (!engineer.IsActive)
            return Result.Failure<VisitDto>("Engineer is not active");

        // Check if engineer has active visit
        var activeVisit = await _visitRepository.GetActiveVisitForEngineerAsync(request.EngineerId, cancellationToken);
        if (activeVisit != null)
            return Result.Failure<VisitDto>("Engineer already has an active visit");

        // Generate visit number
        var visitNumber = await _visitNumberGenerator.GenerateNextVisitNumberAsync(cancellationToken);

        // Create visit
        var visit = Visit.Create(
            visitNumber,
            site.Id,
            site.SiteCode.Value,
            site.Name,
            engineer.Id,
            engineer.Name,
            request.ScheduledDate,
            request.Type);

        // Add supervisor if provided
        if (request.SupervisorId.HasValue)
        {
            var supervisor = await _userRepository.GetByIdAsync(request.SupervisorId.Value, cancellationToken);
            if (supervisor != null && supervisor.Role == UserRole.Supervisor)
            {
                visit.AssignSupervisor(supervisor.Id, supervisor.Name);
            }
        }

        // Add technicians
        foreach (var techName in request.TechnicianNames)
        {
            visit.AddTechnician(techName);
        }

        await _visitRepository.AddAsync(visit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<VisitDto>(visit);
        return Result.Success(dto);
    }
}