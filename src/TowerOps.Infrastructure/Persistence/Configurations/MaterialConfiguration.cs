namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.Materials;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.Code)
            .IsUnique();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.Category)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.OfficeId)
            .IsRequired();

        // ✅ CurrentStock Value Object
        builder.OwnsOne(m => m.CurrentStock, stock =>
        {
            stock.Property(s => s.Value)
                .HasColumnName("CurrentStockValue")
                .HasPrecision(18, 2)
                .IsRequired();

            stock.Property(s => s.Unit)
                .HasColumnName("CurrentStockUnit")
                .HasMaxLength(20)
                .HasConversion<string>()
                .IsRequired();
        });

        // ✅ MinimumStock Value Object
        builder.OwnsOne(m => m.MinimumStock, stock =>
        {
            stock.Property(s => s.Value)
                .HasColumnName("MinimumStockValue")
                .HasPrecision(18, 2)
                .IsRequired();

            stock.Property(s => s.Unit)
                .HasColumnName("MinimumStockUnit")
                .HasMaxLength(20)
                .HasConversion<string>()
                .IsRequired();
        });

        // ✅ ReorderQuantity Value Object (nullable)
        builder.OwnsOne(m => m.ReorderQuantity, stock =>
        {
            stock.Property(s => s.Value)
                .HasColumnName("ReorderQuantityValue")
                .HasPrecision(18, 2);

            stock.Property(s => s.Unit)
                .HasColumnName("ReorderQuantityUnit")
                .HasMaxLength(20)
                .HasConversion<string>();
        });

        // ✅ UnitCost Value Object
        builder.OwnsOne(m => m.UnitCost, money =>
        {
            money.Property(mc => mc.Amount)
                .HasColumnName("UnitCostAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(mc => mc.Currency)
                .HasColumnName("UnitCostCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(m => m.Supplier)
            .HasMaxLength(200);

        builder.Property(m => m.LastRestockDate);

        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ✅ MaterialReservations - Owned Entity Collection
        builder.OwnsMany(m => m.Reservations, reservation =>
        {
            reservation.ToTable("MaterialReservations");

            reservation.WithOwner()
                .HasForeignKey(r => r.MaterialId);

            reservation.HasKey(r => r.Id);

            reservation.Property(r => r.Id)
                .ValueGeneratedNever(); // Guid is set in constructor

            reservation.Property(r => r.MaterialId)
                .IsRequired();

            reservation.Property(r => r.VisitId)
                .IsRequired();

            reservation.Property(r => r.ReservedAt)
                .IsRequired();

            reservation.Property(r => r.IsConsumed)
                .IsRequired()
                .HasDefaultValue(false);

            // Quantity Value Object in Reservation
            reservation.OwnsOne(r => r.Quantity, qty =>
            {
                qty.Property(q => q.Value)
                    .HasColumnName("QuantityValue")
                    .HasPrecision(18, 2)
                    .IsRequired();

                qty.Property(q => q.Unit)
                    .HasColumnName("QuantityUnit")
                    .HasMaxLength(20)
                    .HasConversion<string>()
                    .IsRequired();
            });

            reservation.HasIndex(r => r.VisitId);
            reservation.HasIndex(r => new { r.MaterialId, r.IsConsumed });
        });

        // ✅ MaterialTransactions - Navigation (separate entity, not owned)
        builder.HasMany(m => m.Transactions)
            .WithOne()
            .HasForeignKey(t => t.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit fields
        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.CreatedBy)
            .HasMaxLength(256);

        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(m => m.OfficeId);
        builder.HasIndex(m => m.IsActive);

        // Ignore domain events
        builder.Ignore(m => m.DomainEvents);
    }
}