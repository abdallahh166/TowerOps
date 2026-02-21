using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.Sites;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");

        builder.HasKey(s => s.Id);

        // SiteCode Value Object
        builder.OwnsOne(s => s.SiteCode, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("SiteCode")
                .IsRequired()
                .HasMaxLength(50);

            code.Property(c => c.OfficeCode)
                .HasColumnName("SiteOfficeCode")
                .HasMaxLength(3);

            code.Property(c => c.SequenceNumber)
                .HasColumnName("SiteSequenceNumber");

            code.Property(c => c.ShortCode)
                .HasColumnName("SiteShortCode")
                .HasMaxLength(20)
                .IsRequired(false);

            code.HasIndex(c => c.Value)
                .IsUnique();
        });

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.OMCName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Region)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.SubRegion)
            .IsRequired()
            .HasMaxLength(100);

        // Coordinates Value Object
        builder.OwnsOne(s => s.Coordinates, coords =>
        {
            coords.Property(c => c.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(10, 7);

            coords.Property(c => c.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(10, 7);
        });

        // Address Value Object
        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.Region)
                .HasColumnName("AddressRegion")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.Details)
                .HasColumnName("AddressDetails")
                .HasMaxLength(500);
        });

        builder.Property(s => s.BSCName)
            .HasMaxLength(100);

        builder.Property(s => s.BSCCode)
            .HasMaxLength(50);

        builder.Property(s => s.Subcontractor)
            .HasMaxLength(200);

        builder.Property(s => s.MaintenanceArea)
            .HasMaxLength(100);

        builder.Property(s => s.ZTEMonitoringStatus)
            .HasMaxLength(200);

        builder.Property(s => s.GeneralNotes)
            .HasMaxLength(2000);

        builder.OwnsOne(s => s.RFStatus, rf =>
        {
            rf.Property(r => r.TotalRFCount)
                .HasColumnName("TotalRFCount");

            rf.Property(r => r.RFOnTowerCount)
                .HasColumnName("RFOnTowerCount");

            rf.Property(r => r.RFOnGroundCount)
                .HasColumnName("RFOnGroundCount");

            rf.Property(r => r.RFSectorCarryCount)
                .HasColumnName("RFSectorCarryCount");

            rf.Property(r => r.BandForRFOnTower)
                .HasColumnName("BandForRFOnTower")
                .HasMaxLength(200);

            rf.Property(r => r.BandForRFOnGround)
                .HasColumnName("BandForRFOnGround")
                .HasMaxLength(200);

            rf.Property(r => r.Notes)
                .HasColumnName("RFStatusNotes")
                .HasMaxLength(2000);
        });

        // Relationships - One-to-One with components
        builder.HasOne(s => s.TowerInfo)
            .WithOne()
            .HasForeignKey<SiteTowerInfo>(t => t.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.PowerSystem)
            .WithOne()
            .HasForeignKey<SitePowerSystem>(p => p.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.RadioEquipment)
            .WithOne()
            .HasForeignKey<SiteRadioEquipment>(r => r.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Transmission)
            .WithOne()
            .HasForeignKey<SiteTransmission>(t => t.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.CoolingSystem)
            .WithOne()
            .HasForeignKey<SiteCoolingSystem>(c => c.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.FireSafety)
            .WithOne()
            .HasForeignKey<SiteFireSafety>(f => f.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.SharingInfo)
            .WithOne()
            .HasForeignKey<SiteSharing>(sh => sh.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with User (PM Engineer)
        builder.HasOne(s => s.AssignedEngineer)
            .WithMany(u => u.AssignedSites)
            .HasForeignKey(s => s.AssignedEngineerId)
            .OnDelete(DeleteBehavior.SetNull);


        // Indexes
        builder.HasIndex(s => s.OfficeId);
        builder.HasIndex(s => s.Region);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.Complexity);
        builder.HasIndex(s => s.AssignedEngineerId);
        builder.HasIndex(s => new { s.OfficeId, s.Status });
    }
}
