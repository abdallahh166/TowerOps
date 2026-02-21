namespace TelecomPM.Infrastructure.Persistence.Configurations;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Infrastructure.Persistence;

public sealed class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("ApplicationRoles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(100)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsSystem)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        var comparer = ValueComparerFactory.CreateReadOnlyStringCollectionComparer();
        builder.Property(x => x.Permissions)
            .HasConversion(
                v => SerializePermissions(v),
                v => DeserializePermissions(v))
            .HasColumnType("nvarchar(max)")
            .Metadata.SetValueComparer(comparer);
    }

    private static string SerializePermissions(IReadOnlyCollection<string> permissions)
    {
        return JsonSerializer.Serialize(permissions, JsonSerializerOptions.Default);
    }

    private static List<string> DeserializePermissions(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new List<string>();

        return JsonSerializer.Deserialize<List<string>>(value, JsonSerializerOptions.Default) ?? new List<string>();
    }
}
