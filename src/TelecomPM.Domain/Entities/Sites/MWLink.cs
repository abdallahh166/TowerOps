namespace TelecomPM.Domain.Entities.Sites;

// ==================== MW Link ====================
public sealed class MWLink
{
    public string LinkDirection { get; private set; } = string.Empty;
    public string OppositeSite { get; private set; } = string.Empty;
    public string FrequencyBand { get; private set; } = string.Empty;
    public int DishSizeCM { get; private set; }
    public string ODUType { get; private set; } = string.Empty;

    private MWLink() { }

    public static MWLink Create(
        string linkDirection,
        string oppositeSite,
        string frequencyBand,
        int dishSize,
        string oduType)
    {
        return new MWLink
        {
            LinkDirection = linkDirection,
            OppositeSite = oppositeSite,
            FrequencyBand = frequencyBand,
            DishSizeCM = dishSize,
            ODUType = oduType
        };
    }
}
