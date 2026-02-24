using TelecomPM.Application.DTOs.Portal;

namespace TelecomPM.Application.Common.Interfaces;

public interface IPortalReadRepository
{
    Task<PortalDashboardDto> GetDashboardAsync(string clientCode, CancellationToken cancellationToken = default);

    Task<bool> SiteExistsForClientAsync(
        string clientCode,
        string siteCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PortalSiteDto>> GetSitesAsync(
        string clientCode,
        string? siteCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PortalWorkOrderDto>> GetWorkOrdersAsync(
        string clientCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PortalVisitDto>> GetVisitsAsync(
        string clientCode,
        string siteCode,
        int pageNumber,
        int pageSize,
        bool anonymizeEngineers,
        CancellationToken cancellationToken = default);

    Task<PortalVisitEvidenceDto?> GetVisitEvidenceAsync(
        string clientCode,
        Guid visitId,
        CancellationToken cancellationToken = default);

    Task<PortalSlaReportDto> GetSlaReportAsync(string clientCode, CancellationToken cancellationToken = default);
}
