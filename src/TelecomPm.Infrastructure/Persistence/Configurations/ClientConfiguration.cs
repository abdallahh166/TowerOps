using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Clients;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClientCode).IsRequired().HasMaxLength(32);
        builder.Property(x => x.ClientName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.ContactEmail).HasMaxLength(200);
        builder.Property(x => x.ContactPhone).HasMaxLength(50);

        builder.HasIndex(x => x.ClientCode).IsUnique();
    }
}
