namespace TelecomPM.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using TelecomPM.Domain.Common;
using TelecomPM.Domain.Entities.ApprovalRecords;
using TelecomPM.Domain.Entities.AuditLogs;
using TelecomPM.Domain.Entities.BatteryDischargeTests;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Entities.Escalations;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Interfaces.Services;

public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventDispatcher domainEventDispatcher) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
    }

    // Visit Aggregates
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<VisitPhoto> VisitPhotos => Set<VisitPhoto>();
    public DbSet<VisitReading> VisitReadings => Set<VisitReading>();
    public DbSet<VisitChecklist> VisitChecklists => Set<VisitChecklist>();
    public DbSet<VisitMaterialUsage> VisitMaterialUsages => Set<VisitMaterialUsage>();
    public DbSet<VisitIssue> VisitIssues => Set<VisitIssue>();
    public DbSet<VisitApproval> VisitApprovals => Set<VisitApproval>();

    // Site Aggregates
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<SiteTowerInfo> SiteTowerInfos => Set<SiteTowerInfo>();
    public DbSet<SitePowerSystem> SitePowerSystems => Set<SitePowerSystem>();
    public DbSet<SiteRadioEquipment> SiteRadioEquipments => Set<SiteRadioEquipment>();
    public DbSet<SiteTransmission> SiteTransmissions => Set<SiteTransmission>();
    public DbSet<SiteCoolingSystem> SiteCoolingSystems => Set<SiteCoolingSystem>();
    public DbSet<SiteFireSafety> SiteFireSafeties => Set<SiteFireSafety>();
    public DbSet<SiteSharing> SiteSharings => Set<SiteSharing>();

    // User & Office
    public DbSet<User> Users => Set<User>();
    public DbSet<Office> Offices => Set<Office>();

    // Material
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<MaterialTransaction> MaterialTransactions => Set<MaterialTransaction>();

    // Work Orders
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    // Escalations
    public DbSet<Escalation> Escalations => Set<Escalation>();

    // Audit & Approvals
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ApprovalRecord> ApprovalRecords => Set<ApprovalRecord>();
    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();
    public DbSet<BatteryDischargeTest> BatteryDischargeTests => Set<BatteryDischargeTest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filters (soft delete)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity<Guid>).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Property(parameter, nameof(Entity<Guid>.IsDeleted)),
                    Expression.Constant(false));
                var lambda = Expression.Lambda(body, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        if (domainEvents.Count == 0)
            return;

        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
