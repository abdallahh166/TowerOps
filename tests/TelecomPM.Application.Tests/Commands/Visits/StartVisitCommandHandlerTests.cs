using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TelecomPM.Application.Commands.Visits.StartVisit;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Visits;

public class StartVisitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveTemplate_ShouldAutoGenerateChecklistItems()
    {
        var visit = CreateVisit();
        var template = ChecklistTemplate.Create(VisitType.BM, "v1.0", DateTime.UtcNow, "seed");
        template.AddItem("Power", "Rectifier Visual Check", null, true, 1);
        template.AddItem("Cooling", "A/C Unit 1 Check", null, true, 2);
        template.Activate("manager");

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var templateRepository = new Mock<IChecklistTemplateRepository>();
        templateRepository
            .Setup(r => r.GetActiveByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<VisitDto>(It.IsAny<Visit>())).Returns((Visit source) => new VisitDto
        {
            Id = source.Id,
            VisitNumber = source.VisitNumber,
            SiteId = source.SiteId,
            SiteCode = source.SiteCode,
            SiteName = source.SiteName,
            EngineerId = source.EngineerId,
            EngineerName = source.EngineerName,
            Status = source.Status,
            Type = source.Type
        });

        var logger = new Mock<ILogger<StartVisitCommandHandler>>();

        var handler = new StartVisitCommandHandler(
            visitRepository.Object,
            templateRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            logger.Object);

        var result = await handler.Handle(new StartVisitCommand
        {
            VisitId = visit.Id,
            Latitude = 30.1234,
            Longitude = 31.4321
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        visit.Status.Should().Be(VisitStatus.InProgress);
        visit.ChecklistTemplateId.Should().Be(template.Id);
        visit.ChecklistTemplateVersion.Should().Be("v1.0");
        visit.Checklists.Should().HaveCount(2);
        visit.Checklists.Should().OnlyContain(i => i.TemplateItemId.HasValue);

        visitRepository.Verify(r => r.UpdateAsync(visit, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithoutActiveTemplate_ShouldProceedWithoutError()
    {
        var visit = CreateVisit();

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var templateRepository = new Mock<IChecklistTemplateRepository>();
        templateRepository
            .Setup(r => r.GetActiveByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChecklistTemplate?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<VisitDto>(It.IsAny<Visit>())).Returns((Visit source) => new VisitDto
        {
            Id = source.Id,
            VisitNumber = source.VisitNumber,
            SiteId = source.SiteId,
            SiteCode = source.SiteCode,
            SiteName = source.SiteName,
            EngineerId = source.EngineerId,
            EngineerName = source.EngineerName,
            Status = source.Status,
            Type = source.Type
        });

        var logger = new Mock<ILogger<StartVisitCommandHandler>>();

        var handler = new StartVisitCommandHandler(
            visitRepository.Object,
            templateRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            logger.Object);

        var result = await handler.Handle(new StartVisitCommand
        {
            VisitId = visit.Id,
            Latitude = 30.1234,
            Longitude = 31.4321
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        visit.Status.Should().Be(VisitStatus.InProgress);
        visit.Checklists.Should().BeEmpty();
        visit.ChecklistTemplateId.Should().BeNull();
        visit.ChecklistTemplateVersion.Should().BeNull();

        visitRepository.Verify(r => r.UpdateAsync(visit, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Visit CreateVisit()
        => Visit.Create(
            "V-START-001",
            Guid.NewGuid(),
            "S-TNT-001",
            "Site 1",
            Guid.NewGuid(),
            "Engineer A",
            DateTime.UtcNow.AddHours(1),
            VisitType.PreventiveMaintenance);
}
