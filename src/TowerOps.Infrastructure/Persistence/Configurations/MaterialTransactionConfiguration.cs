namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.Materials;

public class MaterialTransactionConfiguration : IEntityTypeConfiguration<MaterialTransaction>
{
    public void Configure(EntityTypeBuilder<MaterialTransaction> builder)
    {
        builder.ToTable("MaterialTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.PerformedBy)
            .IsRequired()
            .HasMaxLength(200);

        // Owned Type: Quantity
        builder.OwnsOne(t => t.Quantity, qty =>
        {
            qty.Property(q => q.Value)
                .HasColumnName("Quantity")
                .HasPrecision(18, 4);

            qty.Property(q => q.Unit)
                .HasColumnName("Unit")
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Owned Type: StockBefore
        builder.OwnsOne(t => t.StockBefore, qty =>
        {
            qty.Property(q => q.Value)
                .HasColumnName("StockBefore")
                .HasPrecision(18, 4);

            qty.Property(q => q.Unit)
                .HasColumnName("StockBeforeUnit")
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Owned Type: StockAfter
        builder.OwnsOne(t => t.StockAfter, qty =>
        {
            qty.Property(q => q.Value)
                .HasColumnName("StockAfter")
                .HasPrecision(18, 4);

            qty.Property(q => q.Unit)
                .HasColumnName("StockAfterUnit")
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Indexes
        builder.HasIndex(t => t.MaterialId);
        builder.HasIndex(t => t.VisitId);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.TransactionDate);
    }
}