namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Infrastructure.Persistence;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VisitNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.VisitNumber)
            .IsUnique();

        builder.Property(v => v.SiteCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.SiteName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.EngineerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.SupervisorName)
            .HasMaxLength(200);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(v => v.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        var stringListComparer = ValueComparerFactory.CreateStringListComparer();

        builder.Property(v => v.TechnicianNames)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(500)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(v => v.EngineerNotes)
            .HasMaxLength(1000);

        builder.Property(v => v.SupervisorNotes)
            .HasMaxLength(1000);

        builder.Property(v => v.ReviewerNotes)
            .HasMaxLength(1000);

        builder.Property(v => v.ChecklistTemplateVersion)
            .HasMaxLength(32);

        // Owned Type: TimeRange
        builder.OwnsOne(v => v.ActualDuration, duration =>
        {
            duration.Property(d => d.Start)
                .HasColumnName("ActualStartTime");

            duration.Property(d => d.End)
                .HasColumnName("ActualEndTime");
        });

        // Owned Type: Coordinates
        builder.OwnsOne(v => v.CheckInLocation, coords =>
        {
            coords.Property(c => c.Latitude)
                .HasColumnName("CheckInLatitude")
                .HasPrecision(10, 8);

            coords.Property(c => c.Longitude)
                .HasColumnName("CheckInLongitude")
                .HasPrecision(11, 8);
        });

        // Relationships
        builder.HasMany(v => v.Photos)
            .WithOne(p => p.Visit)
            .HasForeignKey(p => p.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Readings)
            .WithOne()
            .HasForeignKey(r => r.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Checklists)
            .WithOne()
            .HasForeignKey(c => c.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.MaterialsUsed)
            .WithOne()
            .HasForeignKey(m => m.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.IssuesFound)
            .WithOne()
            .HasForeignKey(i => i.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.ApprovalHistory)
            .WithOne()
            .HasForeignKey(a => a.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.SiteId);
        builder.HasIndex(v => v.EngineerId);
        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.ScheduledDate);
        builder.HasIndex(v => v.ChecklistTemplateId);

        // Ignore domain events
        builder.Ignore(v => v.DomainEvents);
    }
}
