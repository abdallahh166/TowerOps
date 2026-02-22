using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelecomPM.Domain.Entities.DailyPlans;

namespace TelecomPM.Infrastructure.Persistence.Configurations;

public sealed class DailyPlanConfiguration : IEntityTypeConfiguration<DailyPlan>
{
    public void Configure(EntityTypeBuilder<DailyPlan> builder)
    {
        builder.ToTable("DailyPlans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanDate)
            .HasColumnType("date");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.OfficeId, x.PlanDate }).IsUnique();

        builder.OwnsMany(x => x.EngineerPlans, ep =>
        {
            ep.ToTable("EngineerDayPlans");
            ep.WithOwner().HasForeignKey("DailyPlanId");

            ep.Property(x => x.EngineerId).ValueGeneratedNever();
            ep.HasKey("DailyPlanId", "EngineerId");

            ep.Property(x => x.TotalEstimatedDistanceKm).HasPrecision(10, 3);

            ep.OwnsMany(x => x.Stops, stop =>
            {
                stop.ToTable("PlannedVisitStops");
                stop.WithOwner().HasForeignKey("DailyPlanId", "EngineerId");

                stop.HasKey(x => x.Id);
                stop.Property(x => x.Id).ValueGeneratedNever();
                stop.Property(x => x.SiteCode).HasMaxLength(50).IsRequired();
                stop.Property(x => x.Priority).HasMaxLength(10).IsRequired();
                stop.Property(x => x.VisitType).HasConversion<string>().HasMaxLength(50);
                stop.Property(x => x.DistanceFromPreviousKm).HasPrecision(10, 3);

                stop.OwnsOne(x => x.SiteLocation, geo =>
                {
                    geo.Property(x => x.Latitude).HasColumnName("SiteLatitude").HasPrecision(10, 8);
                    geo.Property(x => x.Longitude).HasColumnName("SiteLongitude").HasPrecision(11, 8);
                });
            });
        });
    }
}
