namespace TowerOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("WorkOrders");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.WoNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(w => w.WoNumber)
            .IsUnique();

        builder.Property(w => w.SiteCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.OfficeCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.IssueDescription)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(w => w.SlaClass)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(w => w.WorkOrderType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasDefaultValue(WorkOrderType.CM);

        builder.Property(w => w.Scope)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40)
            .HasDefaultValue(WorkOrderScope.ClientEquipment);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(w => w.AssignedEngineerName)
            .HasMaxLength(200);

        builder.Property(w => w.AssignedBy)
            .HasMaxLength(200);

        builder.Property(w => w.SlaStartAtUtc)
            .IsRequired();

        builder.Property(w => w.ScheduledVisitDateUtc);

        builder.OwnsOne(w => w.ClientSignature, sig =>
        {
            sig.Property(s => s.SignerName)
                .HasColumnName("ClientSignerName")
                .HasMaxLength(200);

            sig.Property(s => s.SignerRole)
                .HasColumnName("ClientSignerRole")
                .HasMaxLength(100);

            sig.Property(s => s.SignatureDataBase64)
                .HasColumnName("ClientSignatureBase64")
                .HasMaxLength(250000);

            sig.Property(s => s.SignedAtUtc)
                .HasColumnName("ClientSignedAtUtc");

            sig.Property(s => s.SignerPhone)
                .HasColumnName("ClientSignerPhone")
                .HasMaxLength(50);

            sig.OwnsOne(s => s.SignedAtLocation, loc =>
            {
                loc.Property(x => x.Latitude)
                    .HasColumnName("ClientSignedLatitude")
                    .HasPrecision(10, 8);

                loc.Property(x => x.Longitude)
                    .HasColumnName("ClientSignedLongitude")
                    .HasPrecision(11, 8);
            });
        });

        builder.OwnsOne(w => w.EngineerSignature, sig =>
        {
            sig.Property(s => s.SignerName)
                .HasColumnName("EngineerSignerName")
                .HasMaxLength(200);

            sig.Property(s => s.SignerRole)
                .HasColumnName("EngineerSignerRole")
                .HasMaxLength(100);

            sig.Property(s => s.SignatureDataBase64)
                .HasColumnName("EngineerSignatureBase64")
                .HasMaxLength(250000);

            sig.Property(s => s.SignedAtUtc)
                .HasColumnName("EngineerSignedAtUtc");

            sig.Property(s => s.SignerPhone)
                .HasColumnName("EngineerSignerPhone")
                .HasMaxLength(50);

            sig.OwnsOne(s => s.SignedAtLocation, loc =>
            {
                loc.Property(x => x.Latitude)
                    .HasColumnName("EngineerSignedLatitude")
                    .HasPrecision(10, 8);

                loc.Property(x => x.Longitude)
                    .HasColumnName("EngineerSignedLongitude")
                    .HasPrecision(11, 8);
            });
        });

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.SiteCode);
        builder.HasIndex(w => w.OfficeCode);
        builder.HasIndex(w => w.CreatedAt);
        builder.HasIndex(w => w.SlaStartAtUtc);
    }
}
