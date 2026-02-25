using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Services;


namespace TowerOps.Application.Services;

public sealed class SiteAssignmentService : ISiteAssignmentService
{
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;

    public SiteAssignmentService(IUserRepository userRepository, ISiteRepository siteRepository)
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

        if (!engineer.IsActive)
            return false;

        return engineer.CanBeAssignedMoreSites();
    }

    public async Task AssignSiteToEngineerAsync(
        User engineer, 
        Site site, 
        CancellationToken cancellationToken = default)
    {
        if (engineer.Role != UserRole.PMEngineer)
            throw new DomainException("Only PM Engineers can be assigned sites");

        if (!engineer.IsActive)
            throw new DomainException("Engineer is not active");

        if (engineer.OfficeId != site.OfficeId)
            throw new DomainException("Site must belong to the same office as the engineer");

        engineer.AssignSite(site.Id);
    }

    public async Task UnassignSiteFromEngineerAsync(
        User engineer, 
        Guid siteId, 
        CancellationToken cancellationToken = default)
    {
        engineer.UnassignSite(siteId);
    }

    public async Task<List<User>> GetBestEngineersForSiteAsync(
        Site site, 
        CancellationToken cancellationToken = default)
    {
        var engineers = await _userRepository.GetByOfficeIdAsync(site.OfficeId, cancellationToken);
        
        var pmEngineers = engineers
            .Where(e => e.Role == UserRole.PMEngineer && e.IsActive)
            .ToList();

        // Score engineers based on:
        // 1. Available capacity
        // 2. Specialization match
        // 3. Performance rating
        // 4. Current workload

        var scoredEngineers = pmEngineers.Select(e => new
        {
            Engineer = e,
            Score = CalculateEngineerScore(e, site)
        })
        .OrderByDescending(x => x.Score)
        .Select(x => x.Engineer)
        .Take(5)
        .ToList();

        return scoredEngineers;
    }

    private decimal CalculateEngineerScore(User engineer, Site site)
    {
        decimal score = 0;

        // Capacity score (0-30 points)
        if (engineer.MaxAssignedSites.HasValue)
        {
            var utilizationRate = (decimal)engineer.AssignedSiteIds.Count / engineer.MaxAssignedSites.Value;
            score += (1 - utilizationRate) * 30;
        }
        else
        {
            score += 30;
        }

        // Specialization score (0-40 points)
        var siteRequiresGenerator = site.PowerSystem?.HasGenerator ?? false;
        var siteRequiresSolar = site.PowerSystem?.HasSolarPanel ?? false;
        var siteIsShared = site.SharingInfo?.IsShared ?? false;

        if (engineer.Specializations.Contains("Generator Sites") && siteRequiresGenerator)
            score += 15;
        if (engineer.Specializations.Contains("Solar Sites") && siteRequiresSolar)
            score += 15;
        if (engineer.Specializations.Contains("Sharing Sites") && siteIsShared)
            score += 10;
        if (engineer.Specializations.Contains("Complex Sites") && site.Complexity == SiteComplexity.High)
            score += 10;

        // Performance score (0-30 points)
        if (engineer.PerformanceRating.HasValue)
        {
            score += (engineer.PerformanceRating.Value / 5) * 30;
        }

        return score;
    }
}
