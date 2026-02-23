namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;
using TelecomPM.Application.Commands.WorkOrders.RejectByCustomer;
using TelecomPM.Application.Common.Interfaces;
using TelecomPm.Api.Mappings;

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
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(PortalContractMapper.ToPortalSitesQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("sites/{siteCode}")]
    public async Task<IActionResult> GetSiteByCode(string siteCode, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(siteCode.ToPortalSiteByCodeQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("workorders")]
    public async Task<IActionResult> GetWorkOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new object().ToPortalWorkOrdersQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
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
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(siteCode.ToPortalVisitsQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("workorders/{id:guid}/accept")]
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
