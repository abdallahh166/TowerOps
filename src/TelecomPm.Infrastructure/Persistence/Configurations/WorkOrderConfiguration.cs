namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.WorkOrders;

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

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(w => w.AssignedEngineerName)
            .HasMaxLength(200);

        builder.Property(w => w.AssignedBy)
            .HasMaxLength(200);

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.SiteCode);
        builder.HasIndex(w => w.OfficeCode);
        builder.HasIndex(w => w.CreatedAt);
    }
}
