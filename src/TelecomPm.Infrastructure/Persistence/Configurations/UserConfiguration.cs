namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Infrastructure.Persistence;

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

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);
    }
}
