namespace TowerOps.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Contracts.Roles;
using TowerOps.Api.Authorization;
using TowerOps.Application.Commands.Roles.CreateApplicationRole;
using TowerOps.Application.Commands.Roles.DeleteApplicationRole;
using TowerOps.Application.Commands.Roles.UpdateApplicationRole;
using TowerOps.Application.DTOs.Roles;
using TowerOps.Application.Queries.Roles.GetAllApplicationRoles;
using TowerOps.Application.Queries.Roles.GetApplicationRoleById;
using TowerOps.Application.Security;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanManageSettings)]
public sealed class RolesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetAllApplicationRolesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(result.Value.Select(ToResponse).ToList());
    }

    [HttpGet("permissions")]
    public IActionResult GetPermissions()
    {
        return Ok(PermissionConstants.All.OrderBy(p => p).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetApplicationRoleByIdQuery { Id = id },
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(ToResponse(result.Value));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new CreateApplicationRoleCommand
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Description = request.Description,
                IsActive = request.IsActive,
                Permissions = request.Permissions
            },
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                return Conflict(result.Error);

            return HandleResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Name }, ToResponse(result.Value));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpdateApplicationRoleCommand
            {
                Id = id,
                DisplayName = request.DisplayName,
                Description = request.Description,
                IsActive = request.IsActive,
                Permissions = request.Permissions
            },
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(ToResponse(result.Value));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new DeleteApplicationRoleCommand { Id = id },
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error.Contains("system roles cannot be deleted", StringComparison.OrdinalIgnoreCase))
                return BadRequest(result.Error);

            return HandleResult(result);
        }

        return NoContent();
    }

    private static RoleResponse ToResponse(ApplicationRoleDto role)
    {
        return new RoleResponse
        {
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            IsSystem = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = role.Permissions.ToList()
        };
    }
}
