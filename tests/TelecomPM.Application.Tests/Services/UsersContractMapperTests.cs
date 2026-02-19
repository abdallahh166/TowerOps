using FluentAssertions;
using TelecomPm.Api.Contracts.Users;
using TelecomPm.Api.Mappings;
using TelecomPM.Domain.Enums;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class UsersContractMapperTests
{
    [Fact]
    public void ToCommand_FromCreateRequest_MapsFields()
    {
        var officeId = Guid.NewGuid();
        var request = new CreateUserRequest
        {
            Name = "User A",
            Email = "user@example.com",
            PhoneNumber = "01000000000",
            Role = UserRole.PMEngineer,
            OfficeId = officeId,
            MaxAssignedSites = 5,
            Specializations = new List<string> { "Fiber" }
        };

        var command = request.ToCommand();

        command.Name.Should().Be(request.Name);
        command.Email.Should().Be(request.Email);
        command.PhoneNumber.Should().Be(request.PhoneNumber);
        command.Role.Should().Be(request.Role);
        command.OfficeId.Should().Be(request.OfficeId);
        command.MaxAssignedSites.Should().Be(request.MaxAssignedSites);
        command.Specializations.Should().BeEquivalentTo(request.Specializations);
    }

    [Fact]
    public void ToCommand_FromUpdateRequest_MapsFieldsAndRouteUserId()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            Name = "Updated",
            PhoneNumber = "01111111111"
        };

        var command = request.ToCommand(userId);

        command.UserId.Should().Be(userId);
        command.Name.Should().Be(request.Name);
        command.PhoneNumber.Should().Be(request.PhoneNumber);
    }

    [Fact]
    public void ToCommand_FromChangeRoleRequest_MapsFieldsAndRouteUserId()
    {
        var userId = Guid.NewGuid();
        var request = new ChangeUserRoleRequest { NewRole = UserRole.Supervisor };

        var command = request.ToCommand(userId);

        command.UserId.Should().Be(userId);
        command.NewRole.Should().Be(UserRole.Supervisor);
    }

    [Fact]
    public void ToByIdQuery_FromUserId_MapsUserId()
    {
        var userId = Guid.NewGuid();

        var query = userId.ToByIdQuery();

        query.UserId.Should().Be(userId);
    }

    [Fact]
    public void ToDeleteActivateDeactivateCommands_MapRouteIds()
    {
        var userId = Guid.NewGuid();

        var deleteCommand = userId.ToDeleteCommand("admin@example.com");
        var activateCommand = userId.ToActivateCommand();
        var deactivateCommand = userId.ToDeactivateCommand();

        deleteCommand.UserId.Should().Be(userId);
        deleteCommand.DeletedBy.Should().Be("admin@example.com");
        activateCommand.UserId.Should().Be(userId);
        deactivateCommand.UserId.Should().Be(userId);
    }

    [Fact]
    public void ToOfficeQuery_FromOfficeId_MapsOfficeId()
    {
        var officeId = Guid.NewGuid();

        var query = officeId.ToOfficeQuery();

        query.OfficeId.Should().Be(officeId);
    }

    [Fact]
    public void ToQuery_FromRole_MapsRole()
    {
        var query = UserRole.Manager.ToQuery();

        query.Role.Should().Be(UserRole.Manager);
    }

    [Fact]
    public void ToQuery_FromPerformanceInputs_MapsAllFields()
    {
        var userId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow.Date.AddDays(-7);
        var toDate = DateTime.UtcNow.Date;

        var query = userId.ToQuery(fromDate, toDate);

        query.UserId.Should().Be(userId);
        query.FromDate.Should().Be(fromDate);
        query.ToDate.Should().Be(toDate);
    }
}
