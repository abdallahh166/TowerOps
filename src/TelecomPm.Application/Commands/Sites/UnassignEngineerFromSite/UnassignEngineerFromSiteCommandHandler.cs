namespace TelecomPM.Application.Commands.Sites.UnassignEngineerFromSite;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;

public class UnassignEngineerFromSiteCommandHandler : IRequestHandler<UnassignEngineerFromSiteCommand, Result>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnassignEngineerFromSiteCommandHandler(
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnassignEngineerFromSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site == null)
            return Result.Failure("Site not found");

        if (!site.AssignedEngineerId.HasValue)
            return Result.Failure("Site is not assigned to any engineer");

        try
        {
            var engineerId = site.AssignedEngineerId.Value;
            site.UnassignEngineer();

            var engineer = await _userRepository.GetByIdAsync(engineerId, cancellationToken);
            if (engineer != null)
            {
                engineer.UnassignSite(request.SiteId);
                await _userRepository.UpdateAsync(engineer, cancellationToken);
            }

            await _siteRepository.UpdateAsync(site, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (System.Exception ex)
        {
            return Result.Failure($"Failed to unassign engineer: {ex.Message}");
        }
    }
}

