namespace TowerOps.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Authorization;
using TowerOps.Application.Commands.WorkOrders.AcceptByCustomer;
using TowerOps.Application.Commands.WorkOrders.RejectByCustomer;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Api.Mappings;

[ApiController]
[Route("api/portal")]
[Authorize(Policy = ApiAuthorizationPolicies.CanViewPortal)]
public sealed class ClientPortalController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public ClientPortalController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new object().ToPortalDashboardQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("sites")]
    public async Task<IActionResult> GetSites(
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        if (!TryResolveSort(
                sortBy,
                sortDir,
                new[] { "siteCode", "name", "status", "region" },
                defaultSortBy: "siteCode",
                out var resolvedSortBy,
                out var sortDescending,
                out var sortError))
        {
            return sortError!;
        }

        var result = await Mediator.Send(
            PortalContractMapper.ToPortalSitesQuery(safePage, safePageSize, resolvedSortBy, sortDescending),
            cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(result.Value.ToPagedResponse());
    }

    [HttpGet("sites/{siteCode}")]
    public async Task<IActionResult> GetSiteByCode(string siteCode, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(siteCode.ToPortalSiteByCodeQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("workorders")]
    public async Task<IActionResult> GetWorkOrders(
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        if (!TryResolveSort(
                sortBy,
                sortDir,
                new[] { "createdAt", "status", "priority", "siteCode", "slaDeadline" },
                defaultSortBy: "createdAt",
                out var resolvedSortBy,
                out var sortDescending,
                out var sortError))
        {
            return sortError!;
        }

        var result = await Mediator.Send(
            new object().ToPortalWorkOrdersQuery(safePage, safePageSize, resolvedSortBy, sortDescending),
            cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(result.Value.ToPagedResponse());
    }

    [HttpGet("sla-report")]
    public async Task<IActionResult> GetSlaReport(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new object().ToPortalSlaReportQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("visits/{siteCode}")]
    public async Task<IActionResult> GetVisits(
        string siteCode,
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        if (!TryResolveSort(
                sortBy,
                sortDir,
                new[] { "scheduledDate", "status", "type", "visitNumber" },
                defaultSortBy: "scheduledDate",
                out var resolvedSortBy,
                out var sortDescending,
                out var sortError))
        {
            return sortError!;
        }

        var result = await Mediator.Send(
            siteCode.ToPortalVisitsQuery(safePage, safePageSize, resolvedSortBy, sortDescending),
            cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(result.Value.ToPagedResponse());
    }

    [HttpGet("visits/{visitId:guid}/evidence")]
    public async Task<IActionResult> GetVisitEvidence(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitId.ToPortalVisitEvidenceQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("workorders/{id:guid}/accept")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManagePortalWorkOrders)]
    public async Task<IActionResult> AcceptWorkOrder(Guid id, CancellationToken cancellationToken)
    {
        var acceptedBy = string.IsNullOrWhiteSpace(_currentUserService.Email)
            ? _currentUserService.UserId.ToString()
            : _currentUserService.Email;

        var result = await Mediator.Send(new AcceptByCustomerCommand
        {
            WorkOrderId = id,
            AcceptedBy = acceptedBy
        }, cancellationToken);

        return HandleResult(result);
    }

    [HttpPatch("workorders/{id:guid}/reject")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManagePortalWorkOrders)]
    public async Task<IActionResult> RejectWorkOrder(Guid id, [FromBody] PortalRejectWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RejectByCustomerCommand
        {
            WorkOrderId = id,
            Reason = request.Reason
        }, cancellationToken);

        return HandleResult(result);
    }

    public sealed class PortalRejectWorkOrderRequest
    {
        public string Reason { get; init; } = string.Empty;
    }
}
