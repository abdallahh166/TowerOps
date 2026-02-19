namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPm.Api.Contracts.Users;
using TelecomPM.Application.Commands.Users.CreateUser;
using TelecomPM.Application.Commands.Users.UpdateUser;
using TelecomPM.Application.Commands.Users.DeleteUser;
using TelecomPM.Application.Commands.Users.ChangeUserRole;
using TelecomPM.Application.Commands.Users.ActivateUser;
using TelecomPM.Application.Commands.Users.DeactivateUser;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Queries.Users.GetUserById;
using TelecomPM.Application.Queries.Users.GetUsersByOffice;
using TelecomPM.Application.Queries.Users.GetUsersByRole;
using TelecomPM.Application.Queries.Users.GetUserPerformance;
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
        var command = new CreateUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            OfficeId = request.OfficeId,
            MaxAssignedSites = request.MaxAssignedSites,
            Specializations = request.Specializations
        };

        var result = await Mediator.Send(command, cancellationToken);

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
        var query = new GetUserByIdQuery { UserId = userId };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand
        {
            UserId = userId,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand 
        { 
            UserId = userId,
            DeletedBy = ResolveDeletionActor()
        };
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{userId:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid userId,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeUserRoleCommand
        {
            UserId = userId,
            NewRole = request.NewRole
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{userId:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand { UserId = userId };
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand { UserId = userId };
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("office/{officeId:guid}")]
    public async Task<IActionResult> GetByOffice(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersByOfficeQuery { OfficeId = officeId };
        var result = await Mediator.Send(query, cancellationToken);
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

        var query = new GetUsersByRoleQuery { Role = userRole };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{userId:guid}/performance")]
    public async Task<IActionResult> GetPerformance(
        Guid userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query = new GetUserPerformanceQuery
        {
            UserId = userId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = await Mediator.Send(query, cancellationToken);
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
