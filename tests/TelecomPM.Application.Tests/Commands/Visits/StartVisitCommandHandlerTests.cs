using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using TelecomPM.Application.Commands.Visits.StartVisit;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
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

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetByIdAsync(visit.SiteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSiteForVisit(TowerOwnershipType.Host));

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
            siteRepository.Object,
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

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetByIdAsync(visit.SiteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSiteForVisit(TowerOwnershipType.Host));

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
            siteRepository.Object,
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

    [Fact]
    public async Task Handle_HostSite_ShouldGenerateFullChecklistWithoutFiltering()
    {
        var visit = CreateVisit();
        var template = CreateTemplateWith45Items();

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository.Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);

        var templateRepository = new Mock<IChecklistTemplateRepository>();
        templateRepository
            .Setup(r => r.GetActiveByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var site = CreateSiteForVisit(TowerOwnershipType.Host);
        var siteRepository = new Mock<ISiteRepository>();
        siteRepository.Setup(r => r.GetByIdAsync(visit.SiteId, It.IsAny<CancellationToken>())).ReturnsAsync(site);

        var handler = CreateHandler(
            visitRepository,
            templateRepository,
            siteRepository,
            out var unitOfWork);

        var result = await handler.Handle(new StartVisitCommand
        {
            VisitId = visit.Id,
            Latitude = 30.1234,
            Longitude = 31.4321
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        visit.Checklists.Should().HaveCount(45);
        visit.Checklists.Should().NotContain(c => c.ItemName.Contains("host operator", StringComparison.OrdinalIgnoreCase));
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GuestSite_ShouldExcludeSharedInfrastructureItems_AndAddHostReportItem()
    {
        var visit = CreateVisit();
        var template = CreateTemplateWith45Items();

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository.Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);

        var templateRepository = new Mock<IChecklistTemplateRepository>();
        templateRepository
            .Setup(r => r.GetActiveByVisitTypeAsync(VisitType.BM, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        var guestSite = CreateSiteForVisit(TowerOwnershipType.Guest);
        var siteRepository = new Mock<ISiteRepository>();
        siteRepository.Setup(r => r.GetByIdAsync(visit.SiteId, It.IsAny<CancellationToken>())).ReturnsAsync(guestSite);

        var handler = CreateHandler(
            visitRepository,
            templateRepository,
            siteRepository,
            out var unitOfWork);

        var result = await handler.Handle(new StartVisitCommand
        {
            VisitId = visit.Id,
            Latitude = 30.1234,
            Longitude = 31.4321
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        visit.Checklists.Should().NotContain(c => c.Category == ChecklistItemCategory.Tower.ToString());
        visit.Checklists.Should().NotContain(c => c.Category == ChecklistItemCategory.Generator.ToString());
        visit.Checklists.Should().NotContain(c => c.Category == ChecklistItemCategory.Fence.ToString());
        visit.Checklists.Should().NotContain(c => c.Category == ChecklistItemCategory.EarthBar.ToString());
        visit.Checklists.Should().Contain(c => c.ItemName == "Report any tower/shared equipment issues to host operator");
        visit.Checklists.Should().HaveCount(30);

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

    private static Site CreateSiteForVisit(TowerOwnershipType ownershipType)
    {
        var site = Site.Create(
            "TNT001",
            "Site 1",
            "OMC-1",
            Guid.NewGuid(),
            "Region",
            "SubRegion",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        site.SetOwnership(
            ownershipType,
            "Tower Owner",
            "AGR-1",
            "Host Contact",
            "+201111111111");

        return site;
    }

    private static ChecklistTemplate CreateTemplateWith45Items()
    {
        var template = ChecklistTemplate.Create(VisitType.BM, "v1.0", DateTime.UtcNow, "seed");
        var categories = new[]
        {
            ChecklistItemCategory.Power.ToString(),
            ChecklistItemCategory.Cooling.ToString(),
            ChecklistItemCategory.Radio.ToString(),
            ChecklistItemCategory.TX.ToString(),
            ChecklistItemCategory.FireSafety.ToString(),
            ChecklistItemCategory.Tower.ToString(),
            ChecklistItemCategory.Generator.ToString(),
            ChecklistItemCategory.Fence.ToString(),
            ChecklistItemCategory.EarthBar.ToString(),
            ChecklistItemCategory.General.ToString()
        };

        for (var i = 1; i <= 45; i++)
        {
            var category = categories[(i - 1) % categories.Length];
            template.AddItem(category, $"Item {i}", null, true, i);
        }

        template.Activate("manager");
        return template;
    }

    private static StartVisitCommandHandler CreateHandler(
        Mock<IVisitRepository> visitRepository,
        Mock<IChecklistTemplateRepository> templateRepository,
        Mock<ISiteRepository> siteRepository,
        out Mock<IUnitOfWork> unitOfWork)
    {
        unitOfWork = new Mock<IUnitOfWork>();
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
        return new StartVisitCommandHandler(
            visitRepository.Object,
            templateRepository.Object,
            siteRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            logger.Object);
    }
}
