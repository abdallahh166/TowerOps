using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Models; 

public sealed class PhotoChecklistItem
{
    public string ItemName { get; }
    public PhotoCategory Category { get; }
    public bool IsMandatory { get; }

    public PhotoChecklistItem(string itemName, PhotoCategory category, bool isMandatory)
    {
        ItemName = itemName;
        Category = category;
        IsMandatory = isMandatory;
    }
}
