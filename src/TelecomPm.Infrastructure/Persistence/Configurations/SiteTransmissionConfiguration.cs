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
    public class SiteTransmissionConfiguration : IEntityTypeConfiguration<SiteTransmission>
    {
        public void Configure(EntityTypeBuilder<SiteTransmission> builder)
        {
            builder.ToTable("SiteTransmissions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Supplier)
                .IsRequired()
                .HasMaxLength(100);

            // MWLinks as JSON
            builder.OwnsMany(t => t.MWLinks, link =>
            {
                link.ToJson();
                link.Property(l => l.LinkDirection)
                    .HasMaxLength(100);
                link.Property(l => l.OppositeSite)
                    .HasMaxLength(100);
                link.Property(l => l.FrequencyBand)
                    .HasMaxLength(50);
                link.Property(l => l.ODUType)
                    .HasMaxLength(100);
                link.Property(l => l.ManagementIpAddress)
                    .HasMaxLength(100);
                link.Property(l => l.Modulation)
                    .HasMaxLength(100);
                link.Property(l => l.Configuration)
                    .HasMaxLength(100);
                link.Property(l => l.Polarization)
                    .HasMaxLength(50);
                link.Property(l => l.OduSerialNumber)
                    .HasMaxLength(100);
                link.Property(l => l.OppositeOduSerialNumber)
                    .HasMaxLength(100);
                link.Property(l => l.AntennaReference)
                    .HasMaxLength(100);
                link.Property(l => l.TxFrequencyKHz)
                    .HasPrecision(18, 2);
                link.Property(l => l.RxFrequencyKHz)
                    .HasPrecision(18, 2);
                link.Property(l => l.TxPowerDbm)
                    .HasPrecision(18, 2);
                link.Property(l => l.RxPowerDbm)
                    .HasPrecision(18, 2);
                link.Property(l => l.CapacityMbps)
                    .HasPrecision(18, 2);
                link.Property(l => l.TxAzimuth)
                    .HasPrecision(18, 2);
                link.Property(l => l.TxHbaMeters)
                    .HasPrecision(18, 2);
            });

            builder.HasIndex(t => t.SiteId);
        }
    }
}
