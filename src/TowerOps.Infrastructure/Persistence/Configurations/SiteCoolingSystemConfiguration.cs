using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerOps.Domain.Entities.Sites;

namespace TowerOps.Infrastructure.Persistence.Configurations
{
    public class SiteCoolingSystemConfiguration : IEntityTypeConfiguration<SiteCoolingSystem>
    {
        public void Configure(EntityTypeBuilder<SiteCoolingSystem> builder)
        {
            builder.ToTable("SiteCoolingSystems");

            builder.HasKey(c => c.Id);

            // ACUnits as JSON
            builder.OwnsMany(c => c.ACUnits, unit =>
            {
                unit.ToJson();
                unit.Property(u => u.Type)
                    .HasMaxLength(50);
                unit.Property(u => u.SerialNumber)
                    .HasMaxLength(100);
            });

            builder.HasIndex(c => c.SiteId);
        }
    }

}
