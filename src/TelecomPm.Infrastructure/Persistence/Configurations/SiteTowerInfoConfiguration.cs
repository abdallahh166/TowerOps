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
    public class SiteTowerInfoConfiguration : IEntityTypeConfiguration<SiteTowerInfo>
    {
        public void Configure(EntityTypeBuilder<SiteTowerInfo> builder)
        {
            builder.ToTable("SiteTowerInfos");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Owner)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(t => t.SiteId);
        }
    }

}
