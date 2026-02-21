namespace TelecomPM.Infrastructure.Persistence.SeedData;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Entities.SystemSettings;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Security;
using TelecomPM.Infrastructure.Persistence;

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
        offices[0].SetContactInfo("Ahmed Hassan", "+201234567890", "cairo@telecompm.com");
        offices[1].SetContactInfo("Mohamed Ali", "+201234567891", "alex@telecompm.com");
        offices[2].SetContactInfo("Sara Mohamed", "+201234567892", "giza@telecompm.com");

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
                "admin@telecompm.com",
                "+201000000000",
                UserRole.Admin,
                cairoOffice.Id),

            // Managers
            User.Create(
                "Omar Manager",
                "omar.manager@telecompm.com",
                "+201011111111",
                UserRole.Manager,
                cairoOffice.Id),

            User.Create(
                "Fatma Manager",
                "fatma.manager@telecompm.com",
                "+201022222222",
                UserRole.Manager,
                alexOffice.Id),

            // Supervisors
            User.Create(
                "Hassan Supervisor",
                "hassan.supervisor@telecompm.com",
                "+201033333333",
                UserRole.Supervisor,
                cairoOffice.Id),

            // PM Engineers
            User.Create(
                "Ahmed Engineer",
                "ahmed.engineer@telecompm.com",
                "+201044444444",
                UserRole.PMEngineer,
                cairoOffice.Id),

            User.Create(
                "Mona Engineer",
                "mona.engineer@telecompm.com",
                "+201055555555",
                UserRole.PMEngineer,
                cairoOffice.Id),

            User.Create(
                "Khaled Engineer",
                "khaled.engineer@telecompm.com",
                "+201066666666",
                UserRole.PMEngineer,
                alexOffice.Id),

            // Technicians
            User.Create(
                "Mahmoud Technician",
                "mahmoud.tech@telecompm.com",
                "+201077777777",
                UserRole.Technician,
                cairoOffice.Id),

            User.Create(
                "Noha Technician",
                "noha.tech@telecompm.com",
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
                "Cable 16mm²",
                "Power cable 16mm², 3 core",
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
        template.AddItem("Power", "Rectifier Visual Check", null, true, i++);
        template.AddItem("Power", "Battery Visual Check", null, true, i++);
        template.AddItem("Power", "GEDP Check", null, true, i++);
        template.AddItem("Power", "Generator Check", null, true, i++, "[\"GF\"]");
        template.AddItem("Power", "Solar Panel Check", null, true, i++, "[\"GF\"]");
        template.AddItem("Power", "Power Meter Reading", null, true, i++);
        template.AddItem("Power", "CB Status Check", null, true, i++);

        template.AddItem("Cooling", "A/C Unit 1 Check", null, true, i++);
        template.AddItem("Cooling", "A/C Unit 2 Check", null, true, i++, "[\"GF\",\"RT\"]");
        template.AddItem("Cooling", "Ventilation Check", null, false, i++);

        template.AddItem("Radio", "BTS/NodeB Visual Check", null, true, i++);
        template.AddItem("Radio", "Antenna Visual Check", null, true, i++);
        template.AddItem("Radio", "DDF Check", null, true, i++);
        template.AddItem("Radio", "Alarm Status Check", null, true, i++);

        template.AddItem("TX", "MW Link Visual Check", null, true, i++);
        template.AddItem("TX", "ODU Check", null, true, i++);
        template.AddItem("TX", "IP Connectivity Check", null, true, i++);

        template.AddItem("Fire Safety", "Fire Panel Check", null, true, i++);
        template.AddItem("Fire Safety", "Fire Extinguisher Check", null, true, i++);
        template.AddItem("Fire Safety", "Heat Sensor Check", null, true, i++);

        template.AddItem("Structure", "Tower Visual Check", null, true, i++, "[\"GF\"]");
        template.AddItem("Structure", "Fence Check", null, true, i++);
        template.AddItem("Structure", "Earth Bar Check", null, true, i++);
        template.AddItem("Structure", "Shelter Condition Check", null, true, i++);

        template.AddItem("General", "PM Logbook Update", null, true, i++);
        template.AddItem("General", "Site Cleanliness Check", null, true, i++);
        template.AddItem("General", "Pending Issues Review", null, true, i++);
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
            CreateSetting("SLA:P1:ResponseMinutes", "60", "SLA", "int", "Response deadline for P1 in minutes", false, seededBy),
            CreateSetting("SLA:P2:ResponseMinutes", "240", "SLA", "int", "Response deadline for P2 in minutes", false, seededBy),
            CreateSetting("SLA:P3:ResponseMinutes", "1440", "SLA", "int", "Response deadline for P3 in minutes", false, seededBy),
            CreateSetting("SLA:P4:ResponseMinutes", "2880", "SLA", "int", "Response deadline for P4 in minutes", false, seededBy),

            // Evidence
            CreateSetting("Evidence:BM:MinPhotos", "3", "Evidence", "int", "Minimum BM photos", false, seededBy),
            CreateSetting("Evidence:CM:MinPhotos", "2", "Evidence", "int", "Minimum CM photos", false, seededBy),
            CreateSetting("Evidence:BM:ChecklistCompletionPercent", "80", "Evidence", "int", "Minimum BM checklist completion", false, seededBy),
            CreateSetting("Evidence:CM:ChecklistCompletionPercent", "60", "Evidence", "int", "Minimum CM checklist completion", false, seededBy),
            CreateSetting("Evidence:RequiredReadingTypes", "BatteryVoltage,RectifierOutput", "Evidence", "string", "Required reading types", false, seededBy),

            // Company
            CreateSetting("Company:Name", "MobiEgypt", "Company", "string", "Company display name", false, seededBy),
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

            // Import
            CreateSetting("Import:SkipInvalidRows", "true", "Import", "bool", "Skip invalid rows during imports", false, seededBy),
            CreateSetting("Import:MaxRows", "5000", "Import", "int", "Maximum rows per import file", false, seededBy),
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
