using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Roles;

namespace TelecomPM.Application.Queries.Roles.GetApplicationRoleById;

public sealed record GetApplicationRoleByIdQuery : IQuery<ApplicationRoleDto>
{
    public string Id { get; init; } = string.Empty;
}
