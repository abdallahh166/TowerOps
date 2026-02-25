using FluentAssertions;
using Moq;
using TowerOps.Application.Queries.Reports.GetEngineerPerformanceReport;
using TowerOps.Application.Queries.Reports.GetIssueAnalyticsReport;
using TowerOps.Application.Queries.Reports.GetOfficeStatisticsReport;
using TowerOps.Application.Queries.Reports.GetSiteMaintenanceReport;
using TowerOps.Application.Queries.Reports.GetVisitCompletionTrends;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Queries.Reports;

public class ReportQueryEfficiencyTests
{
    [Fact]
    public async Task GetVisitCompletionTrends_ShouldFilterByOfficeSiteIdsUsingSpecification()
    {
        var officeId = Guid.NewGuid();
        var officeSiteId = Guid.NewGuid();
        var otherSiteId = Guid.NewGuid();
        var engineerId = Guid.NewGuid();
        var fromDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);

        var visits = new List<Visit>
        {
            CreateVisit("V-TR-001", officeSiteId, "TNT001", engineerId, fromDate.AddDays(2), VisitType.BM),
            CreateVisit("V-TR-002", otherSiteId, "TNT002", engineerId, fromDate.AddDays(3), VisitType.BM)
        };

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetSiteIdsByOfficeAsNoTrackingAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { officeSiteId });

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var sut = new GetVisitCompletionTrendsQueryHandler(visitRepository.Object, siteRepository.Object);

        var result = await sut.Handle(new GetVisitCompletionTrendsQuery
        {
            OfficeId = officeId,
            FromDate = fromDate,
            ToDate = toDate,
            Period = TrendPeriod.Monthly
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Sum(x => x.TotalVisits).Should().Be(1);
        result.Value.Sum(x => x.TotalVisits).Should().NotBe(visits.Count);

        siteRepository.Verify(r => r.GetSiteIdsByOfficeAsNoTrackingAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetIssueAnalyticsReport_ShouldFilterIssuesByOfficeSiteIdsUsingSpecification()
    {
        var officeId = Guid.NewGuid();
        var officeSiteId = Guid.NewGuid();
        var otherSiteId = Guid.NewGuid();
        var fromDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);

        var officeVisit = CreateVisit("V-IA-001", officeSiteId, "TNT010", Guid.NewGuid(), fromDate.AddDays(1), VisitType.CM);
        var criticalIssue = VisitIssue.Create(officeVisit.Id, IssueCategory.Power, IssueSeverity.Critical, "Rectifier alarm", "Critical alarm");
        var resolvedIssue = VisitIssue.Create(officeVisit.Id, IssueCategory.Cooling, IssueSeverity.Medium, "AC fault", "Cooling degraded");
        resolvedIssue.Resolve("Fixed");
        officeVisit.ReportIssue(criticalIssue);
        officeVisit.ReportIssue(resolvedIssue);

        var otherVisit = CreateVisit("V-IA-002", otherSiteId, "TNT011", Guid.NewGuid(), fromDate.AddDays(2), VisitType.CM);
        otherVisit.ReportIssue(VisitIssue.Create(otherVisit.Id, IssueCategory.Radio, IssueSeverity.High, "TX issue", "External site issue"));

        var visits = new List<Visit> { officeVisit, otherVisit };

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetSiteIdsByOfficeAsNoTrackingAsync(officeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { officeSiteId });

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var sut = new GetIssueAnalyticsReportQueryHandler(visitRepository.Object, siteRepository.Object);

        var result = await sut.Handle(new GetIssueAnalyticsReportQuery
        {
            OfficeId = officeId,
            FromDate = fromDate,
            ToDate = toDate
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalIssues.Should().Be(2);
        result.Value.CriticalIssues.Should().Be(1);

        siteRepository.Verify(r => r.GetSiteIdsByOfficeAsNoTrackingAsync(officeId, It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetOfficeStatisticsReport_ShouldUseOfficeSiteIdsWhenQueryingVisits()
    {
        var officeId = Guid.NewGuid();
        var office = Office.Create("CAI", "Cairo Office", "Cairo", Address.Create("Street", "Cairo", "Cairo"));

        var officeSite = CreateSite("TNT101", officeId, "Office Site");
        var otherSite = CreateSite("TNT102", Guid.NewGuid(), "Other Site");
        var sites = new List<Site> { officeSite, otherSite };

        var engineer = User.Create("Engineer", "eng@test.com", "01000000000", UserRole.PMEngineer, officeId);
        var technician = User.Create("Technician", "tech@test.com", "01000000001", UserRole.Technician, officeId);
        var users = new List<User> { engineer, technician };

        var fromDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);
        var visits = new List<Visit>
        {
            CreateVisit("V-OS-001", officeSite.Id, officeSite.SiteCode.Value, engineer.Id, fromDate.AddDays(5), VisitType.BM),
            CreateVisit("V-OS-002", otherSite.Id, otherSite.SiteCode.Value, engineer.Id, fromDate.AddDays(6), VisitType.BM)
        };

        var materials = new List<Material>
        {
            CreateMaterial("MAT-CAI", officeId),
            CreateMaterial("MAT-OTH", Guid.NewGuid())
        };

        var officeRepository = new Mock<IOfficeRepository>();
        officeRepository.Setup(r => r.GetByIdAsNoTrackingAsync(officeId, It.IsAny<CancellationToken>())).ReturnsAsync(office);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Site> spec, CancellationToken _) => Apply(spec, sites).ToList());

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<User>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<User> spec, CancellationToken _) => Apply(spec, users).ToList());

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var materialRepository = new Mock<IMaterialRepository>();
        materialRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Material>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Material> spec, CancellationToken _) => Apply(spec, materials).ToList());

        var sut = new GetOfficeStatisticsReportQueryHandler(
            officeRepository.Object,
            siteRepository.Object,
            userRepository.Object,
            visitRepository.Object,
            materialRepository.Object);

        var result = await sut.Handle(new GetOfficeStatisticsReportQuery
        {
            OfficeId = officeId,
            FromDate = fromDate,
            ToDate = toDate
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalSites.Should().Be(1);
        result.Value.TotalVisits.Should().Be(1);
        result.Value.TotalMaterials.Should().Be(1);

        visitRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetEngineerPerformanceReport_ShouldApplyDateRangeInSpecification()
    {
        var officeId = Guid.NewGuid();
        var engineer = User.Create("Engineer", "eng2@test.com", "01000000002", UserRole.PMEngineer, officeId);
        var office = Office.Create("ALX", "Alex Office", "Alex", Address.Create("Street", "Alex", "Alex"));

        var fromDate = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 2, 28, 23, 59, 59, DateTimeKind.Utc);

        var visits = new List<Visit>
        {
            CreateVisit("V-EP-001", Guid.NewGuid(), "TNT201", engineer.Id, fromDate.AddDays(2), VisitType.BM),
            CreateVisit("V-EP-002", Guid.NewGuid(), "TNT202", engineer.Id, fromDate.AddMonths(-1).AddDays(1), VisitType.BM)
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(engineer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(engineer);

        var officeRepository = new Mock<IOfficeRepository>();
        officeRepository.Setup(r => r.GetByIdAsync(officeId, It.IsAny<CancellationToken>())).ReturnsAsync(office);

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var sut = new GetEngineerPerformanceReportQueryHandler(
            userRepository.Object,
            visitRepository.Object,
            officeRepository.Object);

        var result = await sut.Handle(new GetEngineerPerformanceReportQuery
        {
            EngineerId = engineer.Id,
            FromDate = fromDate,
            ToDate = toDate
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalVisits.Should().Be(1);

        visitRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.GetByEngineerIdAsNoTrackingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetSiteMaintenanceReport_ShouldApplyDateRangeAtRepositoryLevel()
    {
        var office = Office.Create("TNT", "Tanta Office", "Delta", Address.Create("Street", "Tanta", "Delta"));
        var site = CreateSite("TNT301", office.Id, "Maintenance Site");

        var fromDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2026, 3, 31, 23, 59, 59, DateTimeKind.Utc);
        var visits = new List<Visit>
        {
            CreateVisit("V-SM-001", site.Id, site.SiteCode.Value, Guid.NewGuid(), fromDate.AddDays(3), VisitType.BM),
            CreateVisit("V-SM-002", site.Id, site.SiteCode.Value, Guid.NewGuid(), fromDate.AddMonths(-1).AddDays(3), VisitType.BM)
        };

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.FindOneAsNoTrackingAsync(It.IsAny<ISpecification<Site>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var officeRepository = new Mock<IOfficeRepository>();
        officeRepository.Setup(r => r.GetByIdAsNoTrackingAsync(office.Id, It.IsAny<CancellationToken>())).ReturnsAsync(office);

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ISpecification<Visit> spec, CancellationToken _) => Apply(spec, visits).ToList());

        var userRepository = new Mock<IUserRepository>();

        var sut = new GetSiteMaintenanceReportQueryHandler(
            siteRepository.Object,
            visitRepository.Object,
            officeRepository.Object,
            userRepository.Object);

        var result = await sut.Handle(new GetSiteMaintenanceReportQuery
        {
            SiteId = site.Id,
            FromDate = fromDate,
            ToDate = toDate
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalVisits.Should().Be(1);

        visitRepository.Verify(r => r.FindAsNoTrackingAsync(It.IsAny<ISpecification<Visit>>(), It.IsAny<CancellationToken>()), Times.Once);
        visitRepository.Verify(r => r.GetBySiteIdAsNoTrackingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Site CreateSite(string siteCode, Guid officeId, string name)
    {
        return Site.Create(
            siteCode,
            name,
            "OMC",
            officeId,
            "Cairo",
            "East",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
    }

    private static Visit CreateVisit(
        string visitNumber,
        Guid siteId,
        string siteCode,
        Guid engineerId,
        DateTime scheduledDate,
        VisitType type)
    {
        return Visit.Create(
            visitNumber,
            siteId,
            siteCode,
            $"Site {siteCode}",
            engineerId,
            "Engineer",
            scheduledDate,
            type);
    }

    private static Material CreateMaterial(string code, Guid officeId)
    {
        return Material.Create(
            code,
            "Battery",
            "Battery unit",
            MaterialCategory.Power,
            officeId,
            MaterialQuantity.Create(10, MaterialUnit.Pieces),
            MaterialQuantity.Create(2, MaterialUnit.Pieces),
            Money.Create(100, "EGP"));
    }

    private static IEnumerable<T> Apply<T>(ISpecification<T> specification, IEnumerable<T> source)
    {
        IQueryable<T> query = source.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        IOrderedQueryable<T>? ordered = null;
        if (specification.OrderBy is not null)
        {
            ordered = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            ordered = query.OrderByDescending(specification.OrderByDescending);
        }

        if (ordered is not null)
        {
            foreach (var then in specification.ThenBy)
            {
                ordered = ordered.ThenBy(then);
            }

            foreach (var thenDesc in specification.ThenByDescending)
            {
                ordered = ordered.ThenByDescending(thenDesc);
            }

            query = ordered;
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query.ToList();
    }
}
