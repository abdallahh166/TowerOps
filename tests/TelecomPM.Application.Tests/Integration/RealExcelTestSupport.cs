using ClosedXML.Excel;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Application.Tests.Integration;

internal static class RealExcelTestSupport
{
    public static byte[] LoadExcelBytes(string fileName)
        => File.ReadAllBytes(Path.Combine(GetRepoRoot(), "docs", "excell", fileName));

    public static XLWorkbook OpenWorkbook(string fileName)
        => new(Path.Combine(GetRepoRoot(), "docs", "excell", fileName));

    public static string GetRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var docsPath = Path.Combine(current.FullName, "docs");
            if (Directory.Exists(docsPath))
                return current.FullName;

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root from test runtime directory.");
    }

    public static Site CreateSite(string shortCode, Guid officeId)
    {
        return Site.Create(
            shortCode,
            $"Site {shortCode}",
            $"OMC {shortCode}",
            officeId,
            "Delta",
            "North",
            Coordinates.Create(30.1, 31.2),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
    }

    public static Visit CreateVisit(Site site)
    {
        return Visit.Create(
            $"V-{site.SiteCode.ShortCode ?? site.SiteCode.Value}",
            site.Id,
            site.SiteCode.Value,
            site.Name,
            Guid.NewGuid(),
            "Engineer",
            DateTime.UtcNow.AddDays(1),
            VisitType.BM);
    }

    public static ChecklistTemplate CreateTemplate(VisitType visitType)
    {
        var template = ChecklistTemplate.Create(visitType, "v1.0", DateTime.UtcNow, "seed");
        template.AddItem("General", "Sample Item", null, true, 1);
        template.Activate("approver");
        return template;
    }
}
