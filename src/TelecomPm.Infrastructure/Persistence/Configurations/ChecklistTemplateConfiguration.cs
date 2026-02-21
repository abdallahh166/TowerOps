namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.ChecklistTemplates;

public class ChecklistTemplateConfiguration : IEntityTypeConfiguration<ChecklistTemplate>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplate> builder)
    {
        builder.ToTable("ChecklistTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.VisitType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.EffectiveFromUtc)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(200);

        builder.Property(t => t.ApprovedBy)
            .HasMaxLength(200);

        builder.Property(t => t.ChangeNotes)
            .HasMaxLength(2000);

        builder.HasIndex(t => new { t.VisitType, t.IsActive });
        builder.HasIndex(t => new { t.VisitType, t.EffectiveFromUtc });

        builder.OwnsMany(t => t.Items, item =>
        {
            item.ToTable("ChecklistTemplateItems");
            item.WithOwner().HasForeignKey("ChecklistTemplateId");
            item.HasKey(i => i.Id);

            item.Property(i => i.Id)
                .ValueGeneratedNever();

            item.Property(i => i.Category)
                .HasMaxLength(100);

            item.Property(i => i.ItemName)
                .HasMaxLength(300);

            item.Property(i => i.Description)
                .HasMaxLength(2000);

            item.Property(i => i.ApplicableSiteTypes)
                .HasMaxLength(1000);

            item.Property(i => i.ApplicableVisitTypes)
                .HasMaxLength(1000);

            item.HasIndex(i => i.OrderIndex);
        });

        builder.Navigation(t => t.Items).AutoInclude();
    }
}
