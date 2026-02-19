using FluentAssertions;
using Microsoft.AspNetCore.Http;
using TelecomPm.Api.Contracts.Visits;
using TelecomPm.Api.Mappings;
using TelecomPM.Domain.Enums;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class VisitsContractMapperTests
{
    [Fact]
    public void ToCommand_FromCreateVisitRequest_MapsAllFields()
    {
        var request = new CreateVisitRequest
        {
            SiteId = Guid.NewGuid(),
            EngineerId = Guid.NewGuid(),
            ScheduledDate = DateTime.UtcNow,
            Type = VisitType.CorrectiveMaintenance,
            SupervisorId = Guid.NewGuid(),
            TechnicianNames = new List<string> { "Tech 1" }
        };

        var command = request.ToCommand();

        command.SiteId.Should().Be(request.SiteId);
        command.EngineerId.Should().Be(request.EngineerId);
        command.ScheduledDate.Should().Be(request.ScheduledDate);
        command.Type.Should().Be(request.Type);
        command.SupervisorId.Should().Be(request.SupervisorId);
        command.TechnicianNames.Should().BeEquivalentTo(request.TechnicianNames);
    }

    [Fact]
    public void ToQuery_FromEngineerVisitsParameters_MapsWithRouteId()
    {
        var engineerId = Guid.NewGuid();
        var parameters = new EngineerVisitQueryParameters
        {
            PageNumber = 2,
            PageSize = 50,
            Status = VisitStatus.InProgress,
            From = DateTime.UtcNow.Date.AddDays(-2),
            To = DateTime.UtcNow.Date
        };

        var query = parameters.ToQuery(engineerId);

        query.EngineerId.Should().Be(engineerId);
        query.PageNumber.Should().Be(parameters.PageNumber);
        query.PageSize.Should().Be(parameters.PageSize);
        query.Status.Should().Be(parameters.Status);
        query.From.Should().Be(parameters.From);
        query.To.Should().Be(parameters.To);
    }

    [Fact]
    public void ToCommand_FromAddPhotoRequest_MapsRouteAndFileContext()
    {
        var visitId = Guid.NewGuid();
        var content = new byte[] { 1, 2, 3 };
        var file = new FormFile(new MemoryStream(content), 0, content.Length, "file", "evidence.jpg");
        var request = new AddVisitPhotoRequest
        {
            File = file,
            Type = PhotoType.Before,
            Category = PhotoCategory.Rectifier,
            ItemName = "Rectifier",
            Description = "photo",
            Latitude = 30.1,
            Longitude = 31.2
        };

        using var stream = new MemoryStream(content);
        var command = request.ToCommand(visitId, stream);

        command.VisitId.Should().Be(visitId);
        command.FileName.Should().Be("evidence.jpg");
        command.FileStream.Should().BeSameAs(stream);
        command.Type.Should().Be(request.Type);
        command.Category.Should().Be(request.Category);
        command.ItemName.Should().Be(request.ItemName);
        command.Description.Should().Be(request.Description);
        command.Latitude.Should().Be(request.Latitude);
        command.Longitude.Should().Be(request.Longitude);
    }
}
