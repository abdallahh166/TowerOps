using AutoMapper;
using FluentAssertions;
using Moq;
using System.Reflection;
using TelecomPM.Application.Mappings;
using TelecomPM.Application.Queries.Sites.GetOfficeSites;
using TelecomPM.Application.Queries.Visits.GetEngineerVisits;
using TelecomPM.Application.Queries.Visits.GetVisitsNeedingCorrection;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Specifications;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Queries.Visits;

public class VisitQueryEfficiencyTests
{
    private static readonly IMapper Mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
        .CreateMapper();

    [Fact]
    public async Task GetEngineerVisits_ShouldUseCountAndPagedNoTrackingQueries()
    {
        var engineerId = Guid.NewGuid();
        var baselineDate = DateTime.UtcNow.Date;

        var visits = Enumerable.Range(1, 40)
            .Select(i =>
            {
                var visit = Visit.Create(
                    $"V-EFF-{i:D3}",
                    Guid.NewGuid(),
                    $"TNT{i:D3}",
                    $"Site {i}",
                    i % 2 == 0 ? engineerId : Guid.NewGuid(),
                    "Engineer",
                    baselineDate.AddDays(-i),
                    i % 3 == 0 ? VisitType.CM : VisitType.BM);

                if (i % 5 == 0)
                {
                    visit.Cancel("cancelled");
                }

                return visit;
            })
            .ToList();

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).Count());
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var sut = new GetEngineerVisitsQueryHandler(visitRepository.Object, Mapper);

        var result = await sut.Handle(new GetEngineerVisitsQuery
        {
            EngineerId = engineerId,
            PageNumber = 2,
            PageSize = 7,
            Status = VisitStatus.Scheduled
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.PageNumber.Should().Be(2);
        result.Value.Items.Should().HaveCountLessThanOrEqualTo(7);

        visitRepository.Verify(r => r.CountAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Never);
        visitRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOfficeSites_ShouldUseCountAndPagedNoTrackingQueries()
    {
        var officeId = Guid.NewGuid();

        var sites = Enumerable.Range(1, 30)
            .Select(i =>
            {
                var site = Site.Create(
                    $"TNT{i:D3}",
                    $"Site {i}",
                    "OMC",
                    i <= 20 ? officeId : Guid.NewGuid(),
                    "Cairo",
                    "East",
                    Coordinates.Create(30.0, 31.0),
                    Address.Create("Street", "City", "Region"),
                    SiteType.Macro);

                if (i % 4 == 0)
                {
                    site.UpdateStatus(SiteStatus.OffAir);
                }

                return site;
            })
            .ToList();

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Site> spec, CancellationToken _) => Apply(spec, sites).Count());
        siteRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Site> spec, CancellationToken _) => Apply(spec, sites).ToList());

        var sut = new GetOfficeSitesQueryHandler(siteRepository.Object, Mapper);

        var result = await sut.Handle(new GetOfficeSitesQuery
        {
            OfficeId = officeId,
            PageNumber = 1,
            PageSize = 5,
            Status = SiteStatus.OnAir
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCountLessThanOrEqualTo(5);
        result.Value.TotalCount.Should().BeGreaterThan(0);

        siteRepository.Verify(r => r.CountAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()), Times.Once);
        siteRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()), Times.Once);
        siteRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()), Times.Never);
        siteRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetVisitsNeedingCorrection_WhenEngineerNotSpecified_ShouldReturnAllMatchingVisits()
    {
        var firstEngineer = Guid.NewGuid();
        var secondEngineer = Guid.NewGuid();

        var first = Visit.Create("V-COR-001", Guid.NewGuid(), "TNT901", "Site 1", firstEngineer, "E1", DateTime.UtcNow, VisitType.BM);
        var second = Visit.Create("V-COR-002", Guid.NewGuid(), "TNT902", "Site 2", secondEngineer, "E2", DateTime.UtcNow, VisitType.BM);
        var third = Visit.Create("V-COR-003", Guid.NewGuid(), "TNT903", "Site 3", secondEngineer, "E3", DateTime.UtcNow, VisitType.BM);

        MoveToNeedsCorrection(first);
        MoveToNeedsCorrection(second);
        third.Cancel("excluded");

        var visits = new List<Visit> { first, second, third };

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var sut = new GetVisitsNeedingCorrectionQueryHandler(visitRepository.Object, Mapper);

        var result = await sut.Handle(new GetVisitsNeedingCorrectionQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!.Select(v => v.VisitNumber).Should().Contain(new[] { "V-COR-001", "V-COR-002" });
        visitRepository.Verify(r => r.FindAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static IEnumerable<T> Apply<T>(ISpecification<T> specification, IEnumerable<T> source)
    {
        var query = source.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query.ToList();
    }

    private static void MoveToNeedsCorrection(Visit visit)
    {
        visit.AddPhoto(VisitPhoto.Create(
            visit.Id,
            PhotoType.Before,
            PhotoCategory.Other,
            "Before",
            "before.jpg",
            "/photos/before.jpg"));
        visit.AddPhoto(VisitPhoto.Create(
            visit.Id,
            PhotoType.After,
            PhotoCategory.Other,
            "After",
            "after.jpg",
            "/photos/after.jpg"));
        visit.AddReading(VisitReading.Create(
            visit.Id,
            "Voltage",
            "Power",
            220m,
            "V"));
        var checklist = VisitChecklist.Create(
            visit.Id,
            "General",
            "Check item",
            "Desc",
            isMandatory: true);
        checklist.UpdateStatus(CheckStatus.OK);
        visit.AddChecklistItem(checklist);

        visit.StartVisit(Coordinates.Create(30.1, 31.1));
        var actualStartTimeProperty = typeof(Visit).GetProperty("ActualStartTime", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        actualStartTimeProperty.Should().NotBeNull();
        actualStartTimeProperty!.SetValue(visit, DateTime.UtcNow.AddHours(-1));

        visit.CompleteVisit();
        visit.Submit();
        visit.StartReview();
        visit.RequestCorrection(Guid.NewGuid(), "Reviewer", "Fix checklist");
    }
}
