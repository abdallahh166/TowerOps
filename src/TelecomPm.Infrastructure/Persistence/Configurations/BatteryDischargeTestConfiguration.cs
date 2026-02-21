namespace TelecomPM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.BatteryDischargeTests;

public class BatteryDischargeTestConfiguration : IEntityTypeConfiguration<BatteryDischargeTest>
{
    public void Configure(EntityTypeBuilder<BatteryDischargeTest> builder)
    {
        builder.ToTable("BatteryDischargeTests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SiteCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TestDateUtc)
            .IsRequired();

        builder.Property(x => x.RelatedVisitType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.EngineerName)
            .HasMaxLength(200);

        builder.Property(x => x.SubcontractorOffice)
            .HasMaxLength(200);

        builder.Property(x => x.Network)
            .HasMaxLength(100);

        builder.Property(x => x.SiteCategory)
            .HasMaxLength(100);

        builder.Property(x => x.PowerSource)
            .HasMaxLength(100);

        builder.Property(x => x.StartVoltage)
            .HasPrecision(18, 2);

        builder.Property(x => x.StartAmperage)
            .HasPrecision(18, 2);

        builder.Property(x => x.EndVoltage)
            .HasPrecision(18, 2);

        builder.Property(x => x.EndAmperage)
            .HasPrecision(18, 2);

        builder.Property(x => x.PlvdLlvdValue)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReasonForStop)
            .HasMaxLength(500);

        builder.Property(x => x.ReasonForRepeatedBDT)
            .HasMaxLength(500);

        builder.Property(x => x.CapRequestNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.Week)
            .HasMaxLength(50);

        builder.HasIndex(x => x.SiteId);
        builder.HasIndex(x => x.TestDateUtc);
    }
}
