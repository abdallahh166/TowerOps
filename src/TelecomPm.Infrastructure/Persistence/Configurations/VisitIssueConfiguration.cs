namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Infrastructure.Persistence;

public class VisitIssueConfiguration : IEntityTypeConfiguration<VisitIssue>
{
    public void Configure(EntityTypeBuilder<VisitIssue> builder)
    {
        builder.ToTable("VisitIssues");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Resolution)
            .HasMaxLength(2000);

        builder.Property(i => i.TargetDateUtc);

        var guidListComparer = ValueComparerFactory.CreateGuidListComparer();

        builder.Property(i => i.PhotoIds)
            .HasConversion(
                v => string.Join(',', v.Select(id => id.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(Guid.Parse).ToList())
            .HasMaxLength(2000)
            .Metadata.SetValueComparer(guidListComparer);

        // Indexes
        builder.HasIndex(i => i.VisitId);
        builder.HasIndex(i => i.Severity);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.TargetDateUtc);
    }
}
