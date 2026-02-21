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
    public class SitePowerSystemConfiguration : IEntityTypeConfiguration<SitePowerSystem>
    {
        public void Configure(EntityTypeBuilder<SitePowerSystem> builder)
        {
            builder.ToTable("SitePowerSystems");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.RectifierControllerType)
                .HasMaxLength(100);

            builder.Property(p => p.GeneratorType)
                .HasMaxLength(100);

            builder.Property(p => p.GeneratorSerialNumber)
                .HasMaxLength(100);

            builder.Property(p => p.ElectricityPhaseType)
                .HasMaxLength(50);

            builder.Property(p => p.BatteryBrand)
                .HasMaxLength(100);

            builder.Property(p => p.BatteryHealthStatus)
                .HasMaxLength(100);

            builder.Property(p => p.CabinetVendor)
                .HasMaxLength(100);

            builder.HasIndex(p => p.SiteId);
        }
    }
}
