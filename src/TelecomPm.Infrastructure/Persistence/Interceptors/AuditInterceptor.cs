namespace TelecomPM.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Common;
using TelecomPM.Domain.Interfaces.Services;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public AuditInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditProperties(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditProperties(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditProperties(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUserService.IsAuthenticated 
            ? _currentUserService.UserId.ToString() 
            : "System";
        var now = _dateTimeService.Now;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not Entity<Guid> entity)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    // CreatedAt is already set in constructor, but we ensure CreatedBy is set
                    if (string.IsNullOrEmpty(entity.CreatedBy))
                    {
                        SetPropertyValue(entry, nameof(Entity<Guid>.CreatedBy), userId);
                    }
                    // Ensure UpdatedAt and UpdatedBy are set for new entities
                    SetPropertyValue(entry, nameof(Entity<Guid>.UpdatedAt), now);
                    SetPropertyValue(entry, nameof(Entity<Guid>.UpdatedBy), userId);
                    break;

                case EntityState.Modified:
                    // Update tracking
                    SetPropertyValue(entry, nameof(Entity<Guid>.UpdatedAt), now);
                    SetPropertyValue(entry, nameof(Entity<Guid>.UpdatedBy), userId);
                    break;

                case EntityState.Deleted:
                    // Soft delete: mark as deleted instead of removing
                    entry.State = EntityState.Modified;
                    entity.MarkAsDeleted(userId);
                    SetPropertyValue(entry, nameof(Entity<Guid>.UpdatedAt), now);
                    SetPropertyValue(entry, nameof(Entity<Guid>.UpdatedBy), userId);
                    break;
            }
        }
    }

    private void SetPropertyValue(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string propertyName, object? value)
    {
        var property = entry.Entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        if (property != null && property.CanWrite)
        {
            property.SetValue(entry.Entity, value);
        }
    }
}

