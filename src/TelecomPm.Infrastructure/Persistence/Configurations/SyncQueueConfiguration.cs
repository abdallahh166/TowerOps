using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Sync;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public sealed class SyncQueueConfiguration : IEntityTypeConfiguration<SyncQueue>
{
    public void Configure(EntityTypeBuilder<SyncQueue> builder)
    {
        builder.ToTable("SyncQueues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EngineerId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OperationType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.ConflictReason).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.EngineerId);
        builder.HasIndex(x => x.CreatedOnDeviceUtc);
        builder.HasIndex(x => new { x.DeviceId, x.EngineerId, x.OperationType, x.CreatedOnDeviceUtc })
            .IsUnique();
    }
}
