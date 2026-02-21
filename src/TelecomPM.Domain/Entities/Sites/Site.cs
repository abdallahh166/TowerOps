using TelecomPM.Domain.Common;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events.SiteEvents;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site (Aggregate Root) ====================
public sealed class Site : AggregateRoot<Guid>
{
    public SiteCode SiteCode { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string OMCName { get; private set; } = string.Empty;
    
    // Location
    public Guid OfficeId { get; private set; }
    public string Region { get; private set; } = string.Empty;
    public string SubRegion { get; private set; } = string.Empty;
    public Coordinates Coordinates { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    
    // Classification
    public SiteType SiteType { get; private set; }
    public SiteComplexity Complexity { get; private set; }
    public SiteStatus Status { get; private set; }
    public DateTime? AnnouncementDate { get; private set; }
    
    // BSC Info
    public string BSCName { get; private set; } = string.Empty;
    public string BSCCode { get; private set; } = string.Empty;
    
    // Contractor
    public string Subcontractor { get; private set; } = string.Empty;
    public string MaintenanceArea { get; private set; } = string.Empty;
    public string? ZTEMonitoringStatus { get; private set; }
    public string? GeneralNotes { get; private set; }
    
    // Components (Navigation Properties)
    public SiteTowerInfo TowerInfo { get; private set; } = null!;
    public SitePowerSystem PowerSystem { get; private set; } = null!;
    public SiteRadioEquipment RadioEquipment { get; private set; } = null!;
    public SiteTransmission Transmission { get; private set; } = null!;
    public SiteCoolingSystem CoolingSystem { get; private set; } = null!;
    public SiteFireSafety FireSafety { get; private set; } = null!;
    public SiteSharing? SharingInfo { get; private set; }
    public SiteRFStatus? RFStatus { get; private set; }
    
    // Calculated Fields
    public int EstimatedVisitDurationMinutes { get; private set; }
    public DateTime? LastVisitDate { get; private set; }
    public int RequiredPhotosCount { get; private set; }

    //assigned PM Engineer Id
    public Guid? AssignedEngineerId { get; private set; }
    public User? AssignedEngineer { get; private set; }  // Navigation (اختياري)


    private Site() : base() { } // EF Core

    private Site(
        SiteCode siteCode,
        string name,
        string omcName,
        Guid officeId,
        string region,
        string subRegion,
        Coordinates coordinates,
        Address address,
        SiteType siteType) : base(Guid.NewGuid())
    {
        SiteCode = siteCode;
        Name = name;
        OMCName = omcName;
        OfficeId = officeId;
        Region = region;
        SubRegion = subRegion;
        Coordinates = coordinates;
        Address = address;
        SiteType = siteType;
        Status = SiteStatus.OnAir;
        Complexity = SiteComplexity.Low;
    }

    public static Site Create(
        string siteCode,
        string name,
        string omcName,
        Guid officeId,
        string region,
        string subRegion,
        Coordinates coordinates,
        Address address,
        SiteType siteType)
    {
        var site = new Site(
            SiteCode.Create(siteCode),
            name,
            omcName,
            officeId,
            region,
            subRegion,
            coordinates,
            address,
            siteType);
        site.AddDomainEvent(new SiteCreatedEvent(site.Id, site.SiteCode.Value, site.OfficeId));
        return site;
    }

    public void UpdateBasicInfo(string name, string omcName, SiteType siteType)
    {
        Name = name;
        OMCName = omcName;
        SiteType = siteType;
        MarkAsUpdated("System");
    }

    public void SetBSCInfo(string bscName, string bscCode)
    {
        BSCName = bscName;
        BSCCode = bscCode;
    }

    public void SetContractorInfo(string subcontractor, string maintenanceArea)
    {
        Subcontractor = subcontractor;
        MaintenanceArea = maintenanceArea;
    }

    public void SetMonitoringInfo(string? zteMonitoringStatus, string? generalNotes)
    {
        ZTEMonitoringStatus = zteMonitoringStatus;
        GeneralNotes = generalNotes;
    }

    public void SetTowerInfo(SiteTowerInfo towerInfo)
    {
        TowerInfo = towerInfo;
        RecalculateComplexity();
    }

    public void SetPowerSystem(SitePowerSystem powerSystem)
    {
        PowerSystem = powerSystem;
        RecalculateComplexity();
    }

    public void SetRadioEquipment(SiteRadioEquipment radioEquipment)
    {
        RadioEquipment = radioEquipment;
        RecalculateComplexity();
    }

    public void SetTransmission(SiteTransmission transmission)
    {
        Transmission = transmission;
        RecalculateComplexity();
    }

    public void SetCoolingSystem(SiteCoolingSystem coolingSystem)
    {
        CoolingSystem = coolingSystem;
        RecalculateComplexity();
    }

    public void SetFireSafety(SiteFireSafety fireSafety)
    {
        FireSafety = fireSafety;
        RecalculateComplexity();
    }

    public void SetSharingInfo(SiteSharing sharingInfo)
    {
        SharingInfo = sharingInfo;
        RecalculateComplexity();
    }

    public void SetRFStatus(SiteRFStatus rfStatus)
    {
        RFStatus = rfStatus;
    }

    public void UpdateStatus(SiteStatus status)
    {
        if (Status == status)
            return;

        var old = Status;
        Status = status;
        MarkAsUpdated("System");
        AddDomainEvent(new SiteStatusChangedEvent(Id, old, status));
    }

    public void RecordVisit(DateTime visitDate)
    {
        LastVisitDate = visitDate;
    }

    private void RecalculateComplexity()
    {
        int score = 0;

        // Technology score
        if (RadioEquipment != null)
        {
            if (RadioEquipment.Has2G) score += 10;
            if (RadioEquipment.Has3G) score += 15;
            if (RadioEquipment.Has4G) score += 20;
        }

        // Power system score
        if (PowerSystem != null)
        {
            if (PowerSystem.HasGenerator) score += 15;
            if (PowerSystem.HasSolarPanel) score += 10;
        }

        // Sharing complexity
        if (SharingInfo != null && SharingInfo.IsShared) score += 20;

        // Tower height
        if (TowerInfo != null && TowerInfo.Height > 40) score += 15;

        // Cooling units
        if (CoolingSystem != null) score += CoolingSystem.ACUnitsCount * 5;

        // Transmission
        if (Transmission != null && Transmission.Type == TransmissionType.MW) score += 10;

        // Set complexity based on score
        Complexity = score switch
        {
            > 80 => SiteComplexity.High,
            > 50 => SiteComplexity.Medium,
            _ => SiteComplexity.Low
        };

        // Calculate estimated visit duration
        EstimatedVisitDurationMinutes = CalculateEstimatedDuration(score);
        
        // Calculate required photos
        RequiredPhotosCount = CalculateRequiredPhotos();
    }

    private int CalculateEstimatedDuration(int complexityScore)
    {
        int minutes = 60; // Base time

        if (Complexity == SiteComplexity.High) minutes += 60;
        else if (Complexity == SiteComplexity.Medium) minutes += 30;

        if (RadioEquipment != null)
        {
            if (RadioEquipment.Has2G) minutes += 20;
            if (RadioEquipment.Has3G) minutes += 15;
            if (RadioEquipment.Has4G) minutes += 15;
        }

        if (CoolingSystem != null)
            minutes += CoolingSystem.ACUnitsCount * 10;

        if (PowerSystem?.HasGenerator == true)
            minutes += 20;

        if (SharingInfo?.IsShared == true)
            minutes += 15;

        return minutes;
    }

    private int CalculateRequiredPhotos()
    {
        int count = 20; // Base photos (shelter, tower, earth, etc.)

        if (PowerSystem != null)
        {
            count += 10; // Rectifier, batteries, GEDP
            if (PowerSystem.HasGenerator) count += 5;
            if (PowerSystem.HasSolarPanel) count += 3;
        }

        if (CoolingSystem != null)
            count += CoolingSystem.ACUnitsCount * 4; // Per A/C unit

        if (RadioEquipment != null)
        {
            if (RadioEquipment.Has2G) count += 3;
            if (RadioEquipment.Has3G) count += 3;
            if (RadioEquipment.Has4G) count += 3;
        }

        if (FireSafety != null)
            count += 3;

        if (SharingInfo?.IsShared == true)
            count += 5;

        return count;
    }

    public bool CanBeVisited()
    {
        return Status == SiteStatus.OnAir && !IsDeleted;
    }
    public void AssignToEngineer(Guid engineerId, Guid? assignedBy = null)
    {
        if (AssignedEngineerId.HasValue)
            throw new DomainException("Site is already assigned to an engineer.");

        AssignedEngineerId = engineerId;
        MarkAsUpdated("System");

        AddDomainEvent(new SiteAssignedToEngineerEvent(Id, engineerId, assignedBy ?? Guid.Empty));
    }

    public void UnassignEngineer()
    {
        if (!AssignedEngineerId.HasValue)
            throw new DomainException("Site is not assigned to any engineer.");

        AssignedEngineerId = null;
        MarkAsUpdated("System");
    }

}
