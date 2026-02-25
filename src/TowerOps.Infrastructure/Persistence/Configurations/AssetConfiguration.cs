using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.Assets;

namespace TowerOps.Infrastructure.Persistence.Configurations;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssetCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SiteCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Brand).HasMaxLength(100);
        builder.Property(x => x.Model).HasMaxLength(100);
        builder.Property(x => x.SerialNumber).HasMaxLength(100);

        builder.HasIndex(x => x.AssetCode).IsUnique();
        builder.HasIndex(x => x.SiteCode);
        builder.HasIndex(x => x.Status);

        builder.OwnsMany(x => x.ServiceHistory, history =>
        {
            history.ToTable("AssetServiceRecords");
            history.WithOwner().HasForeignKey("AssetId");
            history.HasKey(x => x.Id);
            history.Property(x => x.Id).ValueGeneratedNever();
            history.Property(x => x.ServiceType).IsRequired().HasMaxLength(50);
            history.Property(x => x.EngineerId).HasMaxLength(100);
            history.Property(x => x.Notes).HasMaxLength(1000);
        });
    }
}
