namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.ApprovalRecords;

public class ApprovalRecordConfiguration : IEntityTypeConfiguration<ApprovalRecord>
{
    public void Configure(EntityTypeBuilder<ApprovalRecord> builder)
    {
        builder.ToTable("ApprovalRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkOrderId)
            .IsRequired();

        builder.Property(x => x.WorkflowType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.StageName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReviewerRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReviewerId)
            .IsRequired();

        builder.Property(x => x.Decision)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Reason)
            .HasMaxLength(2000);

        builder.Property(x => x.DecisionAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.WorkOrderId);
        builder.HasIndex(x => x.DecisionAtUtc);
    }
}
