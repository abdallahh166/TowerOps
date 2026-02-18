using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Services;

namespace TelecomPM.Infrastructure.Services;

public sealed class SiteAssignmentService : ISiteAssignmentService
{
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;

    public SiteAssignmentService(
        IUserRepository userRepository,
        ISiteRepository siteRepository)
    {
        _userRepository = userRepository;
        _siteRepository = siteRepository;
    }

    public async Task<bool> CanAssignSiteToEngineerAsync(
        Guid engineerId,
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var engineer = await _userRepository.GetByIdAsync(engineerId, cancellationToken);
        if (engineer == null || engineer.Role != UserRole.PMEngineer)
            return false;

        if (!engineer.CanBeAssignedMoreSites())
            return false;

        var site = await _siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site == null || !site.CanBeVisited())
            return false;

        return true;
    }

    public async Task AssignSiteToEngineerAsync(
        User engineer,
        Site site,
        CancellationToken cancellationToken = default)
    {
        if (!await CanAssignSiteToEngineerAsync(engineer.Id, site.Id, cancellationToken))
            throw new DomainException(
                "Engineer cannot be assigned to this site");

        engineer.AssignSite(site.Id);
        site.AssignToEngineer(engineer.Id, null);

        await _userRepository.UpdateAsync(engineer, cancellationToken);
        await _siteRepository.UpdateAsync(site, cancellationToken);
    }

    public async Task UnassignSiteFromEngineerAsync(
        User engineer,
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        engineer.UnassignSite(siteId);

        var site = await _siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site != null)
        {
            site.UnassignEngineer();
            await _siteRepository.UpdateAsync(site, cancellationToken);
        }

        await _userRepository.UpdateAsync(engineer, cancellationToken);
    }

    public async Task<List<User>> GetBestEngineersForSiteAsync(
        Site site,
        CancellationToken cancellationToken = default)
    {
        // Get all engineers from the same office
        var engineers = await _userRepository.GetByOfficeIdAsync(
            site.OfficeId,
            cancellationToken);

        // Filter PM Engineers who can take more sites
        return engineers
            .Where(e => e.Role == UserRole.PMEngineer 
                       && e.CanBeAssignedMoreSites())
            .OrderBy(e => e.AssignedSiteIds.Count)
            .ThenByDescending(e => e.PerformanceRating ?? 0)
            .ToList();
    }
}

