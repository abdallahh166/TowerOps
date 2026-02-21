using FluentAssertions;
using TelecomPM.Application.Security;
using TelecomPM.Domain.Enums;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class RolePermissionDefaultsTests
{
    [Fact]
    public void AdminRole_ShouldHaveAllPermissions()
    {
        var permissions = RolePermissionDefaults.GetDefaultPermissions(UserRole.Admin.ToString());

        permissions.Should().BeEquivalentTo(PermissionConstants.All);
    }

    [Fact]
    public void EngineerRole_ShouldNotHaveVisitApprovePermission()
    {
        var permissions = RolePermissionDefaults.GetDefaultPermissions(UserRole.PMEngineer.ToString());

        permissions.Should().NotContain(PermissionConstants.VisitsApprove);
    }

    [Fact]
    public void ManagerRole_ShouldHaveWorkOrderAssignPermission()
    {
        var permissions = RolePermissionDefaults.GetDefaultPermissions(UserRole.Manager.ToString());

        permissions.Should().Contain(PermissionConstants.WorkOrdersAssign);
    }
}
