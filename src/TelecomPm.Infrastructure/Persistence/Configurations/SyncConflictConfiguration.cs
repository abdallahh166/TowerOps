using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Sync;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public sealed class SyncConflictConfiguration : IEntityTypeConfiguration<SyncConflict>
{
    public void Configure(EntityTypeBuilder<SyncConflict> builder)
    {
        builder.ToTable("SyncConflicts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConflictType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Resolution).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.SyncQueueId);

        builder.HasOne<SyncQueue>()
            .WithMany()
            .HasForeignKey(x => x.SyncQueueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
