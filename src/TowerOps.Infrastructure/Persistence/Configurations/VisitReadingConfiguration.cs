namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.Visits;

public class VisitReadingConfiguration : IEntityTypeConfiguration<VisitReading>
{
    public void Configure(EntityTypeBuilder<VisitReading> builder)
    {
        builder.ToTable("VisitReadings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReadingType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Value)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(r => r.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.MinAcceptable)
            .HasPrecision(18, 4);

        builder.Property(r => r.MaxAcceptable)
            .HasPrecision(18, 4);

        builder.Property(r => r.Phase)
            .HasMaxLength(10);

        builder.Property(r => r.Equipment)
            .HasMaxLength(100);

        builder.Property(r => r.Notes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(r => r.VisitId);
        builder.HasIndex(r => r.Category);
        builder.HasIndex(r => r.ReadingType);
    }
}