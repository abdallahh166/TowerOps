using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Entities.Visits;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public class VisitApprovalConfiguration : IEntityTypeConfiguration<VisitApproval>
{
    public void Configure(EntityTypeBuilder<VisitApproval> builder)
    {
        builder.ToTable("VisitApprovals");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ReviewerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Comments)
            .HasMaxLength(1000);

        builder.Property(a => a.ReviewedAt)
            .IsRequired();

        // Relationships are already configured in VisitConfiguration.cs
        // Only define indexes here

        // Indexes
        builder.HasIndex(a => a.VisitId);
        builder.HasIndex(a => a.ReviewerId);
        builder.HasIndex(a => a.Action);
    }
}
