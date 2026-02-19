using FluentAssertions;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Application.Services;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Services;

public class SiteAssignmentServiceTests
{
    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(List<User> users)
        {
            _users = users;
        }

        // ==================== READ OPERATIONS (WITH TRACKING) ====================
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.SingleOrDefault(u => u.Id == id));

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users);

        public Task<IReadOnlyList<User>> FindAsync(TelecomPM.Domain.Specifications.ISpecification<User> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users);

        public Task<User?> FindOneAsync(TelecomPM.Domain.Specifications.ISpecification<User> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<User?>(null);

        // ==================== READ OPERATIONS (WITHOUT TRACKING) ====================
        public Task<User?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.SingleOrDefault(u => u.Id == id));

        public Task<IReadOnlyList<User>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users);

        public Task<IReadOnlyList<User>> FindAsNoTrackingAsync(TelecomPM.Domain.Specifications.ISpecification<User> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users);

        public Task<User?> FindOneAsNoTrackingAsync(TelecomPM.Domain.Specifications.ISpecification<User> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<User?>(null);

        // ==================== QUERY OPERATIONS ====================
        public Task<int> CountAsync(TelecomPM.Domain.Specifications.ISpecification<User> specification, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<bool> ExistsAsync(TelecomPM.Domain.Specifications.ISpecification<User> specification, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        // ==================== WRITE OPERATIONS ====================
        public Task AddAsync(User entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        // ==================== CUSTOM METHODS ====================
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.SingleOrDefault(u => u.Email == email));

        public Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.Role == role).ToList());

        public Task<IReadOnlyList<User>> GetByOfficeIdAsync(Guid officeId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.OfficeId == officeId).ToList());

        public Task<User?> GetByEmailAsNoTrackingAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.SingleOrDefault(u => u.Email == email));

        public Task<IReadOnlyList<User>> GetByRoleAsNoTrackingAsync(UserRole role, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.Role == role).ToList());

        public Task<IReadOnlyList<User>> GetByOfficeIdAsNoTrackingAsync(Guid officeId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.OfficeId == officeId).ToList());

        public Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(!_users.Any(u => u.Email == email && (!excludeUserId.HasValue || u.Id != excludeUserId)));

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Any(u => u.Email == email));

        public Task<int> GetUserCountByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Count(u => u.Role == role));

        public Task<int> GetActiveUserCountByOfficeAsync(Guid officeId, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Count(u => u.OfficeId == officeId && u.IsActive));
    }

    private sealed class FakeSiteRepository : ISiteRepository
    {
        // ==================== READ OPERATIONS (WITH TRACKING) ====================
        public Task<Site?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Site?>(null);

        public Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> FindAsync(TelecomPM.Domain.Specifications.ISpecification<Site> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<Site?> FindOneAsync(TelecomPM.Domain.Specifications.ISpecification<Site> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<Site?>(null);

        public Task<Site?> GetBySiteCodeAsync(string siteCode, CancellationToken cancellationToken = default)
            => Task.FromResult<Site?>(null);

        public Task<IReadOnlyList<Site>> GetByOfficeIdAsync(Guid officeId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByEngineerIdAsync(Guid engineerId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByComplexityAsync(SiteComplexity complexity, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByStatusAsync(SiteStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByRegionAsync(string region, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetBySubRegionAsync(string subRegion, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetUnassignedSitesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetSitesNeedingMaintenanceAsync(int daysThreshold, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        // ==================== READ OPERATIONS (WITHOUT TRACKING) ====================
        public Task<Site?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Site?>(null);

        public Task<IReadOnlyList<Site>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> FindAsNoTrackingAsync(TelecomPM.Domain.Specifications.ISpecification<Site> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<Site?> FindOneAsNoTrackingAsync(TelecomPM.Domain.Specifications.ISpecification<Site> specification, CancellationToken cancellationToken = default)
            => Task.FromResult<Site?>(null);

        public Task<Site?> GetBySiteCodeAsNoTrackingAsync(string siteCode, CancellationToken cancellationToken = default)
            => Task.FromResult<Site?>(null);

        public Task<IReadOnlyList<Site>> GetByOfficeIdAsNoTrackingAsync(Guid officeId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByEngineerIdAsNoTrackingAsync(Guid engineerId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByComplexityAsNoTrackingAsync(SiteComplexity complexity, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByStatusAsNoTrackingAsync(SiteStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetByRegionAsNoTrackingAsync(string region, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetBySubRegionAsNoTrackingAsync(string subRegion, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetUnassignedSitesAsNoTrackingAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        public Task<IReadOnlyList<Site>> GetSitesNeedingMaintenanceAsNoTrackingAsync(int daysThreshold, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Site>>(new List<Site>());

        // ==================== QUERY OPERATIONS ====================
        public Task<int> CountAsync(TelecomPM.Domain.Specifications.ISpecification<Site> specification, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<bool> ExistsAsync(TelecomPM.Domain.Specifications.ISpecification<Site> specification, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> IsSiteCodeUniqueAsync(string siteCode, Guid? excludeSiteId = null, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> CodeExistsAsync(string siteCode, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<int> GetSiteCountByOfficeAsync(Guid officeId, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<int> GetSiteCountByEngineerAsync(Guid engineerId, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<int> GetSiteCountByStatusAsync(SiteStatus status, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<int> GetMaintenanceOverdueCountAsync(int daysThreshold, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<int> GetSiteCountByRegionAsync(string region, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<int> GetUnassignedSitesCountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<bool> HasActiveSitesAsync(Guid engineerId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<int> GetSiteCountByComplexityAsync(SiteComplexity complexity, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        // ==================== WRITE OPERATIONS ====================
        public Task AddAsync(Site entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddRangeAsync(IEnumerable<Site> entities, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task UpdateAsync(Site entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteAsync(Site entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteRangeAsync(IEnumerable<Site> entities, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    [Fact]
    public async Task GetBestEngineersForSiteAsync_ShouldRankByCapacityAndSpecialization()
    {
        // Arrange
        var officeId = Guid.NewGuid();
        var e1 = User.Create("E1", "e1@t.com", "010", UserRole.PMEngineer, officeId);
        var e2 = User.Create("E2", "e2@t.com", "010", UserRole.PMEngineer, officeId);
        var e3 = User.Create("E3", "e3@t.com", "010", UserRole.PMEngineer, officeId);

        e1.SetEngineerCapacity(1, new List<string> { "Generator Sites" });
        e1.AssignSite(Guid.NewGuid()); // full capacity
        e1.UpdatePerformanceRating(5);

        e2.SetEngineerCapacity(5, new List<string> { "Solar Sites", "Sharing Sites" });
        e2.UpdatePerformanceRating(3);

        e3.SetEngineerCapacity(5, new List<string> { "Complex Sites" });
        e3.UpdatePerformanceRating(4);

        var users = new List<User> { e1, e2, e3 };
        var userRepo = new FakeUserRepository(users);
        var siteRepo = new FakeSiteRepository();
        var service = new SiteAssignmentService(userRepo, siteRepo);

        var site = Site.Create(
            "TNT001",
            "Site1",
            "OMC",
            officeId,
            "Cairo",
            "Nasr City",
            Coordinates.Create(30, 31),
            Address.Create("Street", "Cairo", "Cairo"),
            SiteType.Macro);

        var sharing = SiteSharing.Create(Guid.Empty);
        sharing.EnableSharing(string.Empty, new List<string>());
        typeof(Site).GetProperty("SharingInfo")!.SetValue(site, sharing);

        var ps = SitePowerSystem.Create(Guid.Empty, PowerConfiguration.ACOnly, RectifierBrand.Delta, BatteryType.VRLA);
        typeof(Site).GetProperty("PowerSystem")!.SetValue(site, ps);
        typeof(Site).GetProperty("Complexity")!.SetValue(site, SiteComplexity.High);
        typeof(SitePowerSystem).GetMethod("SetSolarPanel")!.Invoke(ps, new object[] { 3000, 10 });

        // Act
        var best = await service.GetBestEngineersForSiteAsync(site);

        // Assert
        best.Should().NotBeEmpty();
        best.First().Id.Should().Be(e2.Id, "solar specialization and capacity should rank high");
    }
}
