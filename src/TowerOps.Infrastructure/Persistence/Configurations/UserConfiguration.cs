namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using TowerOps.Domain.Entities.Users;
using TowerOps.Infrastructure.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // case-insensitive;

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.ClientCode)
            .HasMaxLength(32);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.MustChangePassword)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.FailedLoginAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.LockoutCountInWindow)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.IsManualLockout)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsMfaEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.MfaSecret)
            .HasMaxLength(256);

        var stringListComparer = ValueComparerFactory.CreateReadOnlyStringCollectionComparer();
        var guidListComparer = ValueComparerFactory.CreateReadOnlyGuidCollectionComparer();

        builder.Property(u => u.Specializations)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(500)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(u => u.AssignedSiteIds)
            .HasConversion(
                v => string.Join(',', v.Select(id => id.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(Guid.Parse).ToList())
            .HasMaxLength(5000)
            .Metadata.SetValueComparer(guidListComparer);

        builder.Property(u => u.PerformanceRating)
            .HasPrecision(3, 2);

        // Indexes
        builder.HasIndex(u => u.OfficeId);
        builder.HasIndex(u => u.Role);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.ClientCode);

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);
    }
}
