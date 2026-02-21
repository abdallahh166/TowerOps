using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.BatteryDischargeTests;

public sealed class BatteryDischargeTest : AggregateRoot<Guid>
{
    public Guid SiteId { get; private set; }
    public string SiteCode { get; private set; } = string.Empty;
    public VisitType? RelatedVisitType { get; private set; }
    public string? EngineerName { get; private set; }
    public string? SubcontractorOffice { get; private set; }
    public DateTime TestDateUtc { get; private set; }
    public string? Network { get; private set; }
    public string? SiteCategory { get; private set; }
    public string? PowerSource { get; private set; }
    public int? NodalDegree { get; private set; }
    public decimal? StartVoltage { get; private set; }
    public decimal? StartAmperage { get; private set; }
    public decimal? EndVoltage { get; private set; }
    public decimal? EndAmperage { get; private set; }
    public decimal? PlvdLlvdValue { get; private set; }
    public int? DischargeTimeMinutes { get; private set; }
    public string? ReasonForStop { get; private set; }
    public string? ReasonForRepeatedBDT { get; private set; }
    public string? CapRequestNumber { get; private set; }
    public string? Notes { get; private set; }
    public string? Week { get; private set; }

    private BatteryDischargeTest() : base()
    {
    }

    private BatteryDischargeTest(
        Guid siteId,
        string siteCode,
        DateTime testDateUtc) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        SiteCode = siteCode;
        TestDateUtc = EnsureUtc(testDateUtc);
    }

    public static BatteryDischargeTest Create(Guid siteId, string siteCode, DateTime testDateUtc)
    {
        if (siteId == Guid.Empty)
            throw new DomainException("SiteId is required");

        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("SiteCode is required");

        return new BatteryDischargeTest(siteId, siteCode.Trim(), testDateUtc);
    }

    public void UpdateDetails(
        VisitType? relatedVisitType = null,
        string? engineerName = null,
        string? subcontractorOffice = null,
        string? network = null,
        string? siteCategory = null,
        string? powerSource = null,
        int? nodalDegree = null,
        decimal? startVoltage = null,
        decimal? startAmperage = null,
        decimal? endVoltage = null,
        decimal? endAmperage = null,
        decimal? plvdLlvdValue = null,
        int? dischargeTimeMinutes = null,
        string? reasonForStop = null,
        string? reasonForRepeatedBdt = null,
        string? capRequestNumber = null,
        string? notes = null,
        string? week = null)
    {
        RelatedVisitType = relatedVisitType;
        EngineerName = engineerName;
        SubcontractorOffice = subcontractorOffice;
        Network = network;
        SiteCategory = siteCategory;
        PowerSource = powerSource;
        NodalDegree = nodalDegree;
        StartVoltage = startVoltage;
        StartAmperage = startAmperage;
        EndVoltage = endVoltage;
        EndAmperage = endAmperage;
        PlvdLlvdValue = plvdLlvdValue;
        DischargeTimeMinutes = dischargeTimeMinutes;
        ReasonForStop = reasonForStop;
        ReasonForRepeatedBDT = reasonForRepeatedBdt;
        CapRequestNumber = capRequestNumber;
        Notes = notes;
        Week = week;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
