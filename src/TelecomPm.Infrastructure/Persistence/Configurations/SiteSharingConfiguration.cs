using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Infrastructure.Persistence;

namespace TelecomPM.Infrastructure.Persistence.Configurations
{
    public class SiteSharingConfiguration : IEntityTypeConfiguration<SiteSharing>
    {
        public void Configure(EntityTypeBuilder<SiteSharing> builder)
        {
            builder.ToTable("SiteSharings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.HostOperator)
                .HasMaxLength(100);

            builder.Property(s => s.HostSiteCode)
                .HasMaxLength(50);

            builder.Property(s => s.TxEnclosureType)
                .HasMaxLength(100);

            // GuestOperators as JSON
            var stringListComparer = ValueComparerFactory.CreateStringListComparer();

            builder.Property(s => s.GuestOperators)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(stringListComparer);

            builder.OwnsMany(s => s.AntennaPositions, position =>
            {
                position.ToTable("SiteSharingAntennaPositions");

                position.WithOwner()
                    .HasForeignKey("SiteSharingId");

                position.HasKey(p => p.Id);

                position.Property(p => p.Id)
                    .ValueGeneratedNever();

                position.Property(p => p.Category)
                    .HasMaxLength(20)
                    .IsRequired();

                position.Property(p => p.Index)
                    .IsRequired();

                position.Property(p => p.Azimuth)
                    .HasPrecision(6, 2)
                    .IsRequired();

                position.Property(p => p.HbaMeters)
                    .HasPrecision(6, 2)
                    .IsRequired();
            });

            builder.Navigation(s => s.AntennaPositions).AutoInclude();

            builder.HasIndex(s => s.SiteId);
        }
    }
}
