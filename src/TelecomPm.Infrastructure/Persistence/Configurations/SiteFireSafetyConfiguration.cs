using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelecomPM.Domain.Entities.Sites;

namespace TelecomPm.Infrastructure.Persistence.Configurations
{
    public class SiteFireSafetyConfiguration : IEntityTypeConfiguration<SiteFireSafety>
    {
        public void Configure(EntityTypeBuilder<SiteFireSafety> builder)
        {
            builder.ToTable("SiteFireSafeties");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.FirePanelType)
                .IsRequired()
                .HasMaxLength(100);

            // Extinguishers as JSON
            builder.OwnsMany(f => f.Extinguishers, ext =>
            {
                ext.ToJson();
                ext.Property(e => e.Type)
                    .HasMaxLength(50);
                ext.Property(e => e.Brand)
                    .HasMaxLength(100);
                ext.Property(e => e.SerialNumber)
                    .HasMaxLength(100);
            });

            builder.HasIndex(f => f.SiteId);
        }
    }
}
