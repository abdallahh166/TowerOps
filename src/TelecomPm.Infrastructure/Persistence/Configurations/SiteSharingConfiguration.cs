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

            // GuestOperators as JSON
            var stringListComparer = ValueComparerFactory.CreateStringListComparer();

            builder.Property(s => s.GuestOperators)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(stringListComparer);

            builder.HasIndex(s => s.SiteId);
        }
    }
}
