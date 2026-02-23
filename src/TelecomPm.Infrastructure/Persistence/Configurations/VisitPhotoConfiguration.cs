namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Visits;

public class VisitPhotoConfiguration : IEntityTypeConfiguration<VisitPhoto>
{
    public void Configure(EntityTypeBuilder<VisitPhoto> builder)
    {
        builder.ToTable("VisitPhotos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.ThumbnailPath)
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.CapturedAtUtc);

        // Owned Type: Location
        builder.OwnsOne(p => p.Location, coords =>
        {
            coords.Property(c => c.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(10, 8);

            coords.Property(c => c.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(11, 8);
        });
        builder.Navigation(p => p.Location).IsRequired();

        builder.HasOne(p => p.Visit)
            .WithMany(v => v.Photos)
            .HasForeignKey(p => p.VisitId);

        // Indexes
        builder.HasIndex(p => p.VisitId);
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.Category);
    }
}
