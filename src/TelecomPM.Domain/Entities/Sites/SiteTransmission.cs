using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Transmission ====================
public sealed class SiteTransmission : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public TransmissionType Type { get; private set; }
    public string Supplier { get; private set; } = string.Empty;
    public int LinksCount { get; private set; }
    public bool HasGPS { get; private set; }
    public bool HasADM { get; private set; }
    public bool HasSDH { get; private set; }
    public bool HasEBand { get; private set; }
    public bool HasALURouter { get; private set; }
    public string? NodalDegreeRaw { get; private set; }
    public int? MWLinkCount { get; private set; }
    public List<MWLink> MWLinks { get; private set; } = new();

    private SiteTransmission() : base() { }

    private SiteTransmission(
        Guid siteId,
        TransmissionType type,
        string supplier) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        Type = type;
        Supplier = supplier;
    }

    public static SiteTransmission Create(Guid siteId, TransmissionType type, string supplier)
    {
        return new SiteTransmission(siteId, type, supplier);
    }

    public void SetEquipment(bool gps, bool adm, bool sdh, bool eBand, bool aluRouter)
    {
        HasGPS = gps;
        HasADM = adm;
        HasSDH = sdh;
        HasEBand = eBand;
        HasALURouter = aluRouter;
    }

    public void AddMWLink(MWLink link)
    {
        MWLinks.Add(link);
        LinksCount = MWLinks.Count;
    }

    public void SetNodalDegree(string? nodalDegreeRaw)
    {
        var normalized = string.IsNullOrWhiteSpace(nodalDegreeRaw)
            ? null
            : nodalDegreeRaw.Trim();

        NodalDegreeRaw = normalized;
        MWLinkCount = ParseMWLinkCount(normalized);
    }

    private static int? ParseMWLinkCount(string? nodalDegreeRaw)
    {
        if (string.IsNullOrWhiteSpace(nodalDegreeRaw))
            return null;

        var normalized = nodalDegreeRaw.Replace(" ", string.Empty, StringComparison.Ordinal);

        var plusIndex = normalized.IndexOf('+');
        if (plusIndex > 0)
        {
            var firstPart = normalized[..plusIndex];
            if (int.TryParse(firstPart, out var parsedWithProtection))
                return parsedWithProtection;
        }

        if (int.TryParse(normalized, out var parsedDirect))
            return parsedDirect;

        return null;
    }
}
