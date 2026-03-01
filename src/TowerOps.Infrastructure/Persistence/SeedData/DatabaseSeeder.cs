namespace TowerOps.Infrastructure.Persistence.SeedData;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Entities.ChecklistTemplates;
using TowerOps.Domain.Entities.Clients;
using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.ApplicationRoles;
using TowerOps.Domain.Entities.SystemSettings;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Security;
using TowerOps.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly ISettingsEncryptionService _settingsEncryptionService;

    public DatabaseSeeder(
        ApplicationDbContext context,
        ILogger<DatabaseSeeder> logger,
        ISettingsEncryptionService settingsEncryptionService)
    {
        _context = context;
        _logger = logger;
        _settingsEncryptionService = settingsEncryptionService;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Database should already exist (use migrations)
            // await _context.Database.EnsureCreatedAsync(); // Use migrations instead

            // Seed offices first (required for users, sites, materials)
            if (!await _context.Offices.AnyAsync())
            {
                await SeedOfficesAsync();
                await _context.SaveChangesAsync();
            }

            // Seed users
            if (!await _context.Clients.AnyAsync())
            {
                await SeedClientsAsync();
                await _context.SaveChangesAsync();
            }

            // Seed users
            if (!await _context.Users.AnyAsync())
            {
                await SeedUsersAsync();
                await _context.SaveChangesAsync();
            }

            if (!await _context.ApplicationRoles.AnyAsync())
            {
                await SeedApplicationRolesAsync();
                await _context.SaveChangesAsync();
            }

            // Seed materials
            if (!await _context.Materials.AnyAsync())
            {
                await SeedMaterialsAsync();
                await _context.SaveChangesAsync();
            }

            if (!await _context.ChecklistTemplates.AnyAsync())
            {
                await SeedChecklistTemplatesAsync();
                await _context.SaveChangesAsync();
            }

            if (!await _context.SystemSettings.AnyAsync())
            {
                await SeedSystemSettingsAsync();
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }

    private async Task SeedOfficesAsync()
    {
        var offices = new List<Office>
        {
            Office.Create(
                "CAI",
                "Cairo Office",
                "Cairo",
                Address.Create("Tahrir Square", "Cairo", "Cairo", "Downtown")),
            
            Office.Create(
                "ALX",
                "Alexandria Office",
                "Alexandria",
                Address.Create("Corniche Road", "Alexandria", "Alexandria", "Eastern Harbor")),
            
            Office.Create(
                "GIZ",
                "Giza Office",
                "Giza",
                Address.Create("Pyramids Road", "Giza", "Giza", "Near Pyramids"))
        };

        // Set contact info for offices
        offices[0].SetContactInfo("Ahmed Hassan", "+201234567890", "cairo@towerops.com");
        offices[1].SetContactInfo("Mohamed Ali", "+201234567891", "alex@towerops.com");
        offices[2].SetContactInfo("Sara Mohamed", "+201234567892", "giza@towerops.com");

        await _context.Offices.AddRangeAsync(offices);
        _logger.LogInformation("Seeded {Count} offices", offices.Count);
    }

    private async Task SeedUsersAsync()
    {
        var cairoOffice = await _context.Offices.FirstOrDefaultAsync(o => o.Code == "CAI");
        var alexOffice = await _context.Offices.FirstOrDefaultAsync(o => o.Code == "ALX");

        if (cairoOffice == null || alexOffice == null)
            return;

        var users = new List<User>
        {
            // Admins
            User.Create(
                "System Admin",
                "admin@towerops.com",
                "+201000000000",
                UserRole.Admin,
                cairoOffice.Id),

            // Managers
            User.Create(
                "Omar Manager",
                "omar.manager@towerops.com",
                "+201011111111",
                UserRole.Manager,
                cairoOffice.Id),

            User.Create(
                "Fatma Manager",
                "fatma.manager@towerops.com",
                "+201022222222",
                UserRole.Manager,
                alexOffice.Id),

            // Supervisors
            User.Create(
                "Hassan Supervisor",
                "hassan.supervisor@towerops.com",
                "+201033333333",
                UserRole.Supervisor,
                cairoOffice.Id),

            // PM Engineers
            User.Create(
                "Ahmed Engineer",
                "ahmed.engineer@towerops.com",
                "+201044444444",
                UserRole.PMEngineer,
                cairoOffice.Id),

            User.Create(
                "Mona Engineer",
                "mona.engineer@towerops.com",
                "+201055555555",
                UserRole.PMEngineer,
                cairoOffice.Id),

            User.Create(
                "Khaled Engineer",
                "khaled.engineer@towerops.com",
                "+201066666666",
                UserRole.PMEngineer,
                alexOffice.Id),

            // Technicians
            User.Create(
                "Mahmoud Technician",
                "mahmoud.tech@towerops.com",
                "+201077777777",
                UserRole.Technician,
                cairoOffice.Id),

            User.Create(
                "Noha Technician",
                "noha.tech@towerops.com",
                "+201088888888",
                UserRole.Technician,
                alexOffice.Id)
        };

        // Set engineer capacities
        var engineers = users.Where(u => u.Role == UserRole.PMEngineer).ToList();
        engineers[0].SetEngineerCapacity(10, new List<string> { "Tower Maintenance", "Power Systems" });
        engineers[1].SetEngineerCapacity(8, new List<string> { "Cooling Systems", "Fire Safety" });
        engineers[2].SetEngineerCapacity(12, new List<string> { "Radio Equipment", "Transmission" });

        var hasher = new PasswordHasher<User>();
        const string defaultPassword = "P@ssw0rd123!";
        foreach (var user in users)
        {
            user.SetPassword(defaultPassword, hasher);
        }

        await _context.Users.AddRangeAsync(users);
        _logger.LogInformation("Seeded {Count} users", users.Count);
    }

    private async Task SeedClientsAsync()
    {
        var clients = new List<Client>
        {
            Client.Create("ORANGE", "Orange Egypt"),
            Client.Create("VODAFONE", "Vodafone Egypt"),
            Client.Create("WE", "Telecom Egypt WE"),
            Client.Create("IHS", "IHS Towers")
        };

        await _context.Clients.AddRangeAsync(clients);
        _logger.LogInformation("Seeded {Count} clients", clients.Count);
    }

    private async Task SeedMaterialsAsync()
    {
        var cairoOffice = await _context.Offices.FirstOrDefaultAsync(o => o.Code == "CAI");
        var alexOffice = await _context.Offices.FirstOrDefaultAsync(o => o.Code == "ALX");

        if (cairoOffice == null || alexOffice == null)
            return;

        var materials = new List<Material>
        {
            // Power materials
            Material.Create(
                "BAT-100AH",
                "Battery 100Ah",
                "VRLA Battery 12V 100Ah",
                MaterialCategory.Power,
                cairoOffice.Id,
                MaterialQuantity.Create(50, MaterialUnit.Pieces),
                MaterialQuantity.Create(20, MaterialUnit.Pieces),
                Money.Create(1500, "EGP")),

            Material.Create(
                "CAB-16MM",
                "Cable 16mmÂ²",
                "Power cable 16mmÂ², 3 core",
                MaterialCategory.Cable,
                cairoOffice.Id,
                MaterialQuantity.Create(500, MaterialUnit.Meters),
                MaterialQuantity.Create(200, MaterialUnit.Meters),
                Money.Create(25, "EGP")),

            Material.Create(
                "FUEL-DIESEL",
                "Diesel Fuel",
                "Diesel fuel for generators",
                MaterialCategory.Power,
                cairoOffice.Id,
                MaterialQuantity.Create(1000, MaterialUnit.Liters),
                MaterialQuantity.Create(500, MaterialUnit.Liters),
                Money.Create(12, "EGP")),

            // Cooling materials
            Material.Create(
                "AC-5HP",
                "Air Conditioner 5HP",
                "Split AC unit 5HP",
                MaterialCategory.Cooling,
                alexOffice.Id,
                MaterialQuantity.Create(10, MaterialUnit.Pieces),
                MaterialQuantity.Create(5, MaterialUnit.Pieces),
                Money.Create(15000, "EGP")),

            Material.Create(
                "GAS-R410A",
                "Refrigerant Gas R410A",
                "Refrigerant gas R410A, 13.6kg cylinder",
                MaterialCategory.Cooling,
                alexOffice.Id,
                MaterialQuantity.Create(30, MaterialUnit.Pieces),
                MaterialQuantity.Create(10, MaterialUnit.Pieces),
                Money.Create(800, "EGP"))
        };

        // Set suppliers
        materials[0].SetSupplier("Egyptian Battery Co.");
        materials[1].SetSupplier("Egyptian Cables Ltd.");
        materials[2].SetSupplier("Petroleum Distribution Co.");
        materials[3].SetSupplier("HVAC Solutions");
        materials[4].SetSupplier("Cooling Gases Co.");

        await _context.Materials.AddRangeAsync(materials);
        _logger.LogInformation("Seeded {Count} materials", materials.Count);
    }

    private async Task SeedChecklistTemplatesAsync()
    {
        var createdBy = "SystemSeeder";
        var effectiveFrom = DateTime.UtcNow;

        var bm = ChecklistTemplate.Create(
            VisitType.BM,
            "v1.0",
            effectiveFrom,
            createdBy,
            "Initial BM checklist template");

        AddBmItems(bm);
        bm.Activate(createdBy);

        var cm = ChecklistTemplate.Create(
            VisitType.CM,
            "v1.0",
            effectiveFrom,
            createdBy,
            "Initial CM checklist template");

        AddCmItems(cm);
        cm.Activate(createdBy);

        var audit = ChecklistTemplate.Create(
            VisitType.Audit,
            "v1.0",
            effectiveFrom,
            createdBy,
            "Initial Audit checklist template");

        AddAuditItems(audit);
        audit.Activate(createdBy);

        await _context.ChecklistTemplates.AddRangeAsync(bm, cm, audit);
        _logger.LogInformation("Seeded default checklist templates");
    }

    private static void AddBmItems(ChecklistTemplate template)
    {
        var i = 1;
        template.AddItem(ChecklistItemCategory.Power.ToString(), "Rectifier Visual Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Power.ToString(), "Battery Visual Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Power.ToString(), "GEDP Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Generator.ToString(), "Generator Check", null, true, i++, "[\"GF\"]");
        template.AddItem(ChecklistItemCategory.Power.ToString(), "Solar Panel Check", null, true, i++, "[\"GF\"]");
        template.AddItem(ChecklistItemCategory.Power.ToString(), "Power Meter Reading", null, true, i++);
        template.AddItem(ChecklistItemCategory.Power.ToString(), "CB Status Check", null, true, i++);

        template.AddItem(ChecklistItemCategory.Cooling.ToString(), "A/C Unit 1 Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Cooling.ToString(), "A/C Unit 2 Check", null, true, i++, "[\"GF\",\"RT\"]");
        template.AddItem(ChecklistItemCategory.Cooling.ToString(), "Ventilation Check", null, false, i++);

        template.AddItem(ChecklistItemCategory.Radio.ToString(), "BTS/NodeB Visual Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Radio.ToString(), "Antenna Visual Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Radio.ToString(), "DDF Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.Radio.ToString(), "Alarm Status Check", null, true, i++);

        template.AddItem(ChecklistItemCategory.TX.ToString(), "MW Link Visual Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.TX.ToString(), "ODU Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.TX.ToString(), "IP Connectivity Check", null, true, i++);

        template.AddItem(ChecklistItemCategory.FireSafety.ToString(), "Fire Panel Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.FireSafety.ToString(), "Fire Extinguisher Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.FireSafety.ToString(), "Heat Sensor Check", null, true, i++);

        template.AddItem(ChecklistItemCategory.Tower.ToString(), "Tower Visual Check", null, true, i++, "[\"GF\"]");
        template.AddItem(ChecklistItemCategory.Fence.ToString(), "Fence Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.EarthBar.ToString(), "Earth Bar Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.General.ToString(), "Shelter Condition Check", null, true, i++);

        template.AddItem(ChecklistItemCategory.General.ToString(), "PM Logbook Update", null, true, i++);
        template.AddItem(ChecklistItemCategory.General.ToString(), "Site Cleanliness Check", null, true, i++);
        template.AddItem(ChecklistItemCategory.General.ToString(), "Pending Issues Review", null, true, i++);
    }

    private static void AddCmItems(ChecklistTemplate template)
    {
        var i = 1;
        template.AddItem("Power", "Fault Identification", null, true, i++);
        template.AddItem("Power", "Rectifier Check", null, true, i++);
        template.AddItem("Power", "Battery Check", null, true, i++);
        template.AddItem("Power", "CB Reset/Replace", null, true, i++);

        template.AddItem("Radio", "BTS Alarm Check", null, true, i++);
        template.AddItem("Radio", "Reset/Restore Procedure", null, true, i++);
        template.AddItem("Radio", "Signal Quality Check", null, true, i++);

        template.AddItem("TX", "MW Link Status Check", null, true, i++);
        template.AddItem("TX", "ODU Status Check", null, true, i++);

        template.AddItem("General", "Root Cause Documentation", null, true, i++);
        template.AddItem("General", "Resolution Steps Documented", null, true, i++);
        template.AddItem("General", "Customer Notification", null, true, i++);
    }

    private static void AddAuditItems(ChecklistTemplate template)
    {
        var i = 1;
        template.AddItem("SQI", "Network Audit Checklist", null, true, i++);
        template.AddItem("SQI", "RF Status Verification", null, true, i++);
        template.AddItem("SQI", "Documentation Completeness", null, true, i++);
        template.AddItem("SQI", "Compliance Check", null, true, i++);

        template.AddItem("All", "Evidence Package Complete", null, true, i++);
        template.AddItem("All", "Photos Adequate", null, true, i++);
        template.AddItem("All", "Readings Verified", null, true, i++);
    }

    private async Task SeedSystemSettingsAsync()
    {
        const string seededBy = "SystemSeeder";

        var settings = new List<SystemSetting>
        {
            // SLA
            CreateSetting("SLA:CM:P1:ResponseMinutes", "60", "SLA", "int", "CM response deadline for P1 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P2:ResponseMinutes", "240", "SLA", "int", "CM response deadline for P2 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P3:ResponseMinutes", "1440", "SLA", "int", "CM response deadline for P3 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P4:ResponseMinutes", "2880", "SLA", "int", "CM response deadline for P4 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P1:ResolutionMinutes", "240", "SLA", "int", "CM resolution deadline for P1 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P2:ResolutionMinutes", "480", "SLA", "int", "CM resolution deadline for P2 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P3:ResolutionMinutes", "1440", "SLA", "int", "CM resolution deadline for P3 in minutes", false, seededBy),
            CreateSetting("SLA:CM:P4:ResolutionMinutes", "2880", "SLA", "int", "CM resolution deadline for P4 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P1:ResponseMinutes", "60", "SLA", "int", "PM response deadline for P1 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P2:ResponseMinutes", "240", "SLA", "int", "PM response deadline for P2 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P3:ResponseMinutes", "1440", "SLA", "int", "PM response deadline for P3 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P4:ResponseMinutes", "2880", "SLA", "int", "PM response deadline for P4 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P1:ResolutionMinutes", "240", "SLA", "int", "PM resolution deadline for P1 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P2:ResolutionMinutes", "480", "SLA", "int", "PM resolution deadline for P2 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P3:ResolutionMinutes", "1440", "SLA", "int", "PM resolution deadline for P3 in minutes", false, seededBy),
            CreateSetting("SLA:PM:P4:ResolutionMinutes", "2880", "SLA", "int", "PM resolution deadline for P4 in minutes", false, seededBy),
            CreateSetting("SLA:P1:ResponseMinutes", "60", "SLA", "int", "Response deadline for P1 in minutes", false, seededBy),
            CreateSetting("SLA:P2:ResponseMinutes", "240", "SLA", "int", "Response deadline for P2 in minutes", false, seededBy),
            CreateSetting("SLA:P3:ResponseMinutes", "1440", "SLA", "int", "Response deadline for P3 in minutes", false, seededBy),
            CreateSetting("SLA:P4:ResponseMinutes", "2880", "SLA", "int", "Response deadline for P4 in minutes", false, seededBy),
            CreateSetting("SLA:AtRiskThresholdPercent", "80", "SLA", "int", "Legacy at-risk threshold as percent of response window elapsed", false, seededBy),
            CreateSetting("SLA:CM:AtRiskThresholdPercent", "80", "SLA", "int", "CM at-risk threshold as percent of response window elapsed", false, seededBy),
            CreateSetting("SLA:PM:AtRiskThresholdPercent", "80", "SLA", "int", "PM at-risk threshold as percent of response window elapsed", false, seededBy),
            CreateSetting("SLA:Evaluation:Enabled", "true", "SLA", "bool", "Enable background SLA status evaluation", false, seededBy),
            CreateSetting("SLA:Evaluation:IntervalSeconds", "60", "SLA", "int", "SLA evaluation polling interval in seconds", false, seededBy),
            CreateSetting("SLA:Evaluation:BatchSize", "200", "SLA", "int", "Number of open work orders evaluated per cycle", false, seededBy),

            // Evidence
            CreateSetting("Evidence:BM:MinPhotos", "3", "Evidence", "int", "Minimum BM photos", false, seededBy),
            CreateSetting("Evidence:CM:MinPhotos", "2", "Evidence", "int", "Minimum CM photos", false, seededBy),
            CreateSetting("Evidence:BM:ChecklistCompletionPercent", "80", "Evidence", "int", "Minimum BM checklist completion", false, seededBy),
            CreateSetting("Evidence:CM:ChecklistCompletionPercent", "60", "Evidence", "int", "Minimum CM checklist completion", false, seededBy),
            CreateSetting("Evidence:RequiredReadingTypes", "BatteryVoltage,RectifierOutput", "Evidence", "string", "Required reading types", false, seededBy),

            // Company
            CreateSetting("Company:Name", "Seven Pictures - TowerOps", "Company", "string", "Company display name", false, seededBy),
            CreateSetting("Company:DefaultTimezone", "Africa/Cairo", "Company", "string", "Default company timezone", false, seededBy),
            CreateSetting("Company:LogoUrl", string.Empty, "Company", "string", "Public logo URL", false, seededBy),

            // Notifications (encrypted)
            CreateSetting("Notifications:Twilio:AccountSid", string.Empty, "Notifications", "secret", "Twilio account SID", true, seededBy),
            CreateSetting("Notifications:Twilio:AuthToken", string.Empty, "Notifications", "secret", "Twilio auth token", true, seededBy),
            CreateSetting("Notifications:Twilio:FromNumber", string.Empty, "Notifications", "secret", "Twilio sender number", true, seededBy),
            CreateSetting("Notifications:Firebase:ServerKey", string.Empty, "Notifications", "secret", "Firebase server key", true, seededBy),
            CreateSetting("Notifications:Email:SmtpHost", string.Empty, "Notifications", "secret", "SMTP host", true, seededBy),
            CreateSetting("Notifications:Email:SmtpPort", "587", "Notifications", "secret", "SMTP port", true, seededBy),
            CreateSetting("Notifications:Email:Username", string.Empty, "Notifications", "secret", "SMTP username", true, seededBy),
            CreateSetting("Notifications:Email:Password", string.Empty, "Notifications", "secret", "SMTP password", true, seededBy),
            CreateSetting("Notifications:Email:FromAddress", string.Empty, "Notifications", "secret", "SMTP from address", true, seededBy),

            // GPS
            CreateSetting("GPS:AllowedRadiusMeters", "200", "GPS", "int", "Default allowed check-in radius in meters", false, seededBy),
            CreateSetting("GPS:BlockCheckInOutsideRadius", "false", "GPS", "bool", "Block check-in when outside allowed radius", false, seededBy),

            // Sync
            CreateSetting("Sync:MaxBatchSize", "50", "Sync", "int", "Maximum sync items per batch", false, seededBy),
            CreateSetting("Sync:MaxRetries", "3", "Sync", "int", "Maximum retries before marking sync as failed", false, seededBy),
            CreateSetting("Sync:ConflictResolutionMode", "ServerWins", "Sync", "string", "Default conflict resolution mode", false, seededBy),

            // Portal
            CreateSetting("Portal:EnableClientPortal", "true", "Portal", "bool", "Enable read-only client portal", false, seededBy),
            CreateSetting("Portal:DataRetentionDays", "90", "Portal", "int", "Portal data retention in days", false, seededBy),
            CreateSetting("Portal:AnonymizeEngineers", "true", "Portal", "bool", "Show anonymized engineer names in portal", false, seededBy),

            // Route
            CreateSetting("Route:AverageSpeedKmh", "40", "Route", "int", "Average travel speed for route estimation", false, seededBy),
            CreateSetting("Route:MaxSitesPerEngineerPerDay", "8", "Route", "int", "Maximum planned sites per engineer per day", false, seededBy),
            CreateSetting("Route:EnableRamadanScheduling", "true", "Route", "bool", "Enable Ramadan reduced daily assignment capacity", false, seededBy),
            CreateSetting("Route:RamadanMaxSitesPerEngineerPerDay", "6", "Route", "int", "Maximum planned sites per engineer per day during Ramadan", false, seededBy),
            CreateSetting("Route:EnableKhamsinSeasonAdjustment", "true", "Route", "bool", "Enable khamsin route speed adjustment window", false, seededBy),
            CreateSetting("Route:KhamsinStartMonthDay", "03-01", "Route", "string", "Khamsin season start month-day (MM-dd)", false, seededBy),
            CreateSetting("Route:KhamsinEndMonthDay", "05-15", "Route", "string", "Khamsin season end month-day (MM-dd)", false, seededBy),
            CreateSetting("Route:KhamsinAverageSpeedKmh", "30", "Route", "int", "Average travel speed during khamsin season", false, seededBy),

            // Asset
            CreateSetting("Asset:WarrantyAlertDaysBeforeExpiry", "30", "Asset", "int", "Days before warranty expiry alert", false, seededBy),
            CreateSetting("Asset:AutoRegisterFromImport", "true", "Asset", "bool", "Auto-register assets during import", false, seededBy),

            // Upload Security
            CreateSetting("UploadSecurity:QuarantineContainer", "quarantine", "UploadSecurity", "string", "Container/prefix for newly uploaded files pending malware scan", false, seededBy),
            CreateSetting("UploadSecurity:MalwareScan:Provider", "ClamAV", "UploadSecurity", "string", "Malware scanner provider (ClamAV or AzureDefender)", false, seededBy),
            CreateSetting("UploadSecurity:ClamAV:Host", string.Empty, "UploadSecurity", "string", "ClamAV host for INSTREAM scanning", false, seededBy),
            CreateSetting("UploadSecurity:ClamAV:Port", "3310", "UploadSecurity", "int", "ClamAV TCP port", false, seededBy),
            CreateSetting("UploadSecurity:ClamAV:TimeoutSeconds", "10", "UploadSecurity", "int", "ClamAV scan timeout in seconds", false, seededBy),
            CreateSetting("UploadSecurity:Scan:Enabled", "true", "UploadSecurity", "bool", "Enable upload malware scan background worker", false, seededBy),
            CreateSetting("UploadSecurity:Scan:IntervalSeconds", "60", "UploadSecurity", "int", "Upload scan polling interval seconds", false, seededBy),
            CreateSetting("UploadSecurity:Scan:BatchSize", "100", "UploadSecurity", "int", "Pending uploads scanned per cycle", false, seededBy),

            // Privacy / retention
            CreateSetting("Privacy:Retention:OperationalYears", "5", "Privacy", "int", "Operational data minimum retention in years", false, seededBy),
            CreateSetting("Privacy:Retention:SignatureYears", "7", "Privacy", "int", "Signature evidence retention in years", false, seededBy),
            CreateSetting("Privacy:Retention:AuditLogYears", "7", "Privacy", "int", "Audit log retention in years", false, seededBy),
            CreateSetting("Privacy:Retention:SoftDeleteGraceDays", "90", "Privacy", "int", "Soft-delete grace period before hard purge", false, seededBy),
            CreateSetting("Privacy:Retention:CleanupEnabled", "true", "Privacy", "bool", "Enable background retention cleanup worker", false, seededBy),
            CreateSetting("Privacy:Retention:CleanupIntervalHours", "24", "Privacy", "int", "Retention cleanup polling interval in hours", false, seededBy),
            CreateSetting("Privacy:Retention:CleanupBatchSize", "200", "Privacy", "int", "Maximum records processed per cleanup cycle", false, seededBy),
            CreateSetting("Privacy:Jurisdiction:Strategy", "Strictest", "Privacy", "string", "Applies strictest retention policy among active client contracts", false, seededBy),
            CreateSetting("Privacy:Export:TtlDays", "30", "Privacy", "int", "Days an export request remains downloadable", false, seededBy),
            CreateSetting("Privacy:Export:MaxItemsPerCollection", "2000", "Privacy", "int", "Maximum items included per collection in user export payload", false, seededBy),

            // Import
            CreateSetting("Import:SkipInvalidRows", "true", "Import", "bool", "Skip invalid rows during imports", false, seededBy),
            CreateSetting("Import:MaxRows", "5000", "Import", "int", "Maximum rows per import file", false, seededBy),
            CreateSetting("Import:MaxFileSizeBytes", "10485760", "Import", "int", "Maximum import file size in bytes", false, seededBy),
            CreateSetting("Import:DefaultDateFormat", "dd/MM/yyyy", "Import", "string", "Default import date format", false, seededBy)
        };

        await _context.SystemSettings.AddRangeAsync(settings);
        _logger.LogInformation("Seeded {Count} system settings", settings.Count);
    }

    private async Task SeedApplicationRolesAsync()
    {
        var roles = new List<ApplicationRole>
        {
            ApplicationRole.Create(
                UserRole.Admin.ToString(),
                "Administrator",
                "System administrator with full access.",
                isSystem: true,
                isActive: true,
                RolePermissionDefaults.GetDefaultPermissions(UserRole.Admin.ToString())),

            ApplicationRole.Create(
                UserRole.Manager.ToString(),
                "Business Manager",
                "Business manager role.",
                isSystem: false,
                isActive: true,
                RolePermissionDefaults.GetDefaultPermissions(UserRole.Manager.ToString())),

            ApplicationRole.Create(
                UserRole.Supervisor.ToString(),
                "Supervisor",
                "Supervises field operations.",
                isSystem: false,
                isActive: true,
                RolePermissionDefaults.GetDefaultPermissions(UserRole.Supervisor.ToString())),

            ApplicationRole.Create(
                UserRole.PMEngineer.ToString(),
                "Field Engineer",
                "Performs site visits and submissions.",
                isSystem: true,
                isActive: true,
                RolePermissionDefaults.GetDefaultPermissions(UserRole.PMEngineer.ToString())),

            ApplicationRole.Create(
                UserRole.Technician.ToString(),
                "Technician",
                "Field technician with read/assist access.",
                isSystem: false,
                isActive: true,
                RolePermissionDefaults.GetDefaultPermissions(UserRole.Technician.ToString())),

            ApplicationRole.Create(
                "Viewer",
                "Viewer",
                "Read-only access profile.",
                isSystem: false,
                isActive: true,
                new[]
                {
                    PermissionConstants.SitesView,
                    PermissionConstants.VisitsView,
                    PermissionConstants.WorkOrdersView,
                    PermissionConstants.ReportsView,
                    PermissionConstants.KpiView,
                    PermissionConstants.MaterialsView
                }),

            ApplicationRole.Create(
                "ClientPortal",
                "Client Portal",
                "Read-only portal access for tower owner clients.",
                isSystem: false,
                isActive: true,
                new[]
                {
                    PermissionConstants.PortalViewSites,
                    PermissionConstants.PortalViewVisits,
                    PermissionConstants.PortalViewKpis,
                    PermissionConstants.PortalViewWorkOrders,
                    PermissionConstants.PortalViewSla
                })
        };

        await _context.ApplicationRoles.AddRangeAsync(roles);
        _logger.LogInformation("Seeded {Count} application roles", roles.Count);
    }

    private SystemSetting CreateSetting(
        string key,
        string value,
        string group,
        string dataType,
        string description,
        bool isEncrypted,
        string updatedBy)
    {
        var storedValue = isEncrypted
            ? _settingsEncryptionService.Encrypt(value)
            : value;

        return SystemSetting.Create(
            key,
            storedValue,
            group,
            dataType,
            description,
            isEncrypted,
            updatedBy);
    }
}
