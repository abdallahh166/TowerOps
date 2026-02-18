namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Visits;

public class VisitMaterialUsageConfiguration : IEntityTypeConfiguration<VisitMaterialUsage>
{
    public void Configure(EntityTypeBuilder<VisitMaterialUsage> builder)
    {
        builder.ToTable("VisitMaterialUsages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MaterialCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.MaterialName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Owned Type: MaterialQuantity
        builder.OwnsOne(m => m.Quantity, quantity =>
        {
            quantity.Property(q => q.Value)
                .HasColumnName("Quantity")
                .HasPrecision(18, 4);

            quantity.Property(q => q.Unit)
                .HasColumnName("Unit")
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Owned Type: Money (UnitCost)
        builder.OwnsOne(m => m.UnitCost, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitCost")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(10);
        });

        // Owned Type: Money (TotalCost)
        builder.OwnsOne(m => m.TotalCost, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalCost")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("TotalCostCurrency")
                .HasMaxLength(10);
        });

        // Indexes
        builder.HasIndex(m => m.VisitId);
        builder.HasIndex(m => m.MaterialId);
        builder.HasIndex(m => m.Status);
    }
}