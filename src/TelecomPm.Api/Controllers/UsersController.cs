namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPm.Api.Contracts.Users;
using TelecomPm.Api.Mappings;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public UsersController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { userId = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(userId.ToByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(userId.ToDeleteCommand(ResolveDeletionActor()), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{userId:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid userId,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{userId:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(userId.ToActivateCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(userId.ToDeactivateCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("office/{officeId:guid}")]
    public async Task<IActionResult> GetByOffice(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToOfficeQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("role/{role}")]
    public async Task<IActionResult> GetByRole(
        string role,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            return BadRequest($"Invalid role: {role}");
        }

        var result = await Mediator.Send(userRole.ToQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{userId:guid}/performance")]
    public async Task<IActionResult> GetPerformance(
        Guid userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(userId.ToQuery(fromDate, toDate), cancellationToken);
        return HandleResult(result);
    }

    private string ResolveDeletionActor()
    {
        if (_currentUserService.IsAuthenticated)
        {
            if (!string.IsNullOrWhiteSpace(_currentUserService.Email))
            {
                return _currentUserService.Email;
            }

            if (_currentUserService.UserId != Guid.Empty)
            {
                return _currentUserService.UserId.ToString();
            }
        }

        return "System";
    }
}
