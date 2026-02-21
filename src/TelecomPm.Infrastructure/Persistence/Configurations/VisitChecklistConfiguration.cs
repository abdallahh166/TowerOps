namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Infrastructure.Persistence;

public class VisitChecklistConfiguration : IEntityTypeConfiguration<VisitChecklist>
{
    public void Configure(EntityTypeBuilder<VisitChecklist> builder)
    {
        builder.ToTable("VisitChecklists");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Notes)
            .HasMaxLength(500);

        var guidListComparer = ValueComparerFactory.CreateGuidListComparer();

        builder.Property(c => c.RelatedPhotoIds)
            .HasConversion(
                v => v != null ? string.Join(',', v.Select(id => id.ToString())) : string.Empty,
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<Guid>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .Metadata.SetValueComparer(guidListComparer);

        // Indexes
        builder.HasIndex(c => c.VisitId);
        builder.HasIndex(c => c.Category);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.TemplateItemId);
    }
}
