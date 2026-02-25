namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.Escalations;

public class EscalationConfiguration : IEntityTypeConfiguration<Escalation>
{
    public void Configure(EntityTypeBuilder<Escalation> builder)
    {
        builder.ToTable("Escalations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.IncidentId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.IncidentId)
            .IsUnique();

        builder.Property(e => e.SiteCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.SlaClass)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(e => e.Level)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(e => e.EvidencePackage)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.PreviousActions)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.RecommendedDecision)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.SubmittedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(e => e.WorkOrderId);
        builder.HasIndex(e => e.Level);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.SubmittedAtUtc);
    }
}
