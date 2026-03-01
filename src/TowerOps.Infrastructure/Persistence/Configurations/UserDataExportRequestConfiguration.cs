using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.UserDataExports;

namespace TowerOps.Infrastructure.Persistence.Configurations;

public sealed class UserDataExportRequestConfiguration : IEntityTypeConfiguration<UserDataExportRequest>
{
    public void Configure(EntityTypeBuilder<UserDataExportRequest> builder)
    {
        builder.ToTable("UserDataExportRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.RequestedAtUtc)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.PayloadJson);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAtUtc);
        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
