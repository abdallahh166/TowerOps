using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.UnusedAssets;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public sealed class UnusedAssetConfiguration : IEntityTypeConfiguration<UnusedAsset>
{
    public void Configure(EntityTypeBuilder<UnusedAsset> builder)
    {
        builder.ToTable("UnusedAssets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssetName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.RecordedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.SiteId);
        builder.HasIndex(x => x.VisitId);
    }
}
