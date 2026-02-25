namespace TowerOps.Application.Commands.Sites.AssignEngineerToSite;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class AssignEngineerToSiteCommandHandler : IRequestHandler<AssignEngineerToSiteCommand, Result<SiteDto>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssignEngineerToSiteCommandHandler(
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SiteDto>> Handle(AssignEngineerToSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure<SiteDto>("Site not found");

        var engineer = await _userRepository.GetByIdAsync(request.EngineerId, cancellationToken);
        if (engineer == null)
            return Result.Failure<SiteDto>("Engineer not found");

        if (engineer.Role != UserRole.PMEngineer)
            return Result.Failure<SiteDto>("User is not a PM Engineer");

        if (!engineer.IsActive)
            return Result.Failure<SiteDto>("Engineer is not active");

        // Check engineer capacity
        if (!engineer.CanBeAssignedMoreSites())
            return Result.Failure<SiteDto>("Engineer has reached maximum site capacity");

        try
        {
            site.AssignToEngineer(request.EngineerId, request.AssignedBy);
            engineer.AssignSite(request.SiteId);

            await _siteRepository.UpdateAsync(site, cancellationToken);
            await _userRepository.UpdateAsync(engineer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<SiteDto>(site);
            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<SiteDto>($"Failed to assign engineer: {ex.Message}");
        }
    }
}

