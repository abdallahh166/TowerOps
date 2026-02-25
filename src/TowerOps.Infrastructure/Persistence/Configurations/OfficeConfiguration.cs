namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.Offices;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.ToTable("Offices");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(o => o.Code)
            .IsUnique();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Region)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.ContactPerson)
            .HasMaxLength(200);

        builder.Property(o => o.ContactPhone)
            .HasMaxLength(50);

        builder.Property(o => o.ContactEmail)
            .HasMaxLength(200);

        // Owned Type: Address
        builder.OwnsOne(o => o.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("City")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.Region)
                .HasColumnName("AddressRegion")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.Details)
                .HasColumnName("AddressDetails")
                .HasMaxLength(500);
        });

        // Owned Type: Coordinates (optional)
        builder.OwnsOne(o => o.Coordinates, coords =>
        {
            coords.Property(c => c.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(10, 8);

            coords.Property(c => c.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(11, 8);
        });

        // Indexes
        builder.HasIndex(o => o.Region);
        builder.HasIndex(o => o.IsActive);

        // Ignore domain events
        builder.Ignore(o => o.DomainEvents);
    }
}