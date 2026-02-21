namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Contracts.Roles;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Security;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Interfaces.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RolesController : ApiControllerBase
{
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RolesController(
        IApplicationRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsNoTrackingAsync(cancellationToken);
        return Ok(roles.OrderBy(r => r.Name).Select(ToResponse).ToList());
    }

    [HttpGet("permissions")]
    public IActionResult GetPermissions()
    {
        return Ok(PermissionConstants.All.OrderBy(p => p).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsNoTrackingAsync(id, cancellationToken);
        if (role is null)
            return NotFound();

        return Ok(ToResponse(role));
    }

    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSettings)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Role name is required.");

        var existing = await _roleRepository.GetByIdAsync(name, cancellationToken);
        if (existing is not null)
            return Conflict($"Role '{name}' already exists.");

        var role = ApplicationRole.Create(
            name,
            request.DisplayName,
            request.Description,
            isSystem: false,
            isActive: request.IsActive,
            request.Permissions);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, ToResponse(role));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSettings)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role is null)
            return NotFound();

        role.Update(
            request.DisplayName,
            request.Description,
            request.IsActive,
            request.Permissions);

        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(role));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSettings)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role is null)
            return NotFound();

        if (!role.CanBeDeleted())
            return BadRequest("System roles cannot be deleted.");

        await _roleRepository.DeleteAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static RoleResponse ToResponse(ApplicationRole role)
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
