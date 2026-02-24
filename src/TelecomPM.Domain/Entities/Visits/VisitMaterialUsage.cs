using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.Visits;

// ==================== Visit Material Usage ====================
public sealed class VisitMaterialUsage : Entity<Guid>
{
    public Guid VisitId { get; private set; }
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; } = string.Empty;
    public string MaterialName { get; private set; } = string.Empty;
    public MaterialCategory Category { get; private set; }
    public MaterialQuantity Quantity { get; private set; } = null!;
    public Money UnitCost { get; private set; } = null!;
    public Money TotalCost { get; private set; } = null!;
    public string Reason { get; private set; } = string.Empty;
    public Guid? BeforePhotoId { get; private set; }
    public Guid? AfterPhotoId { get; private set; }
    public DateTime UsedAt { get; private set; }
    public MaterialUsageStatus Status { get; private set; }

    private VisitMaterialUsage() : base() { }

    private VisitMaterialUsage(
        Guid visitId,
        Guid materialId,
        string materialCode,
        string materialName,
        MaterialCategory category,
        MaterialQuantity quantity,
        Money unitCost,
        string reason) : base(Guid.NewGuid())
    {
        VisitId = visitId;
        MaterialId = materialId;
        MaterialCode = materialCode;
        MaterialName = materialName;
        Category = category;
        Quantity = quantity;
        UnitCost = unitCost;
        TotalCost = Money.Create(unitCost.Amount * quantity.Value, "EGP");
        Reason = reason;
        UsedAt = DateTime.UtcNow;
        Status = MaterialUsageStatus.Logged;
    }

    public static VisitMaterialUsage Create(
        Guid visitId,
        Guid materialId,
        string materialCode,
        string materialName,
        MaterialCategory category,
        MaterialQuantity quantity,
        Money unitCost,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Material usage reason is required", "VisitMaterialUsage.Reason.Required");

        return new VisitMaterialUsage(
            visitId, materialId, materialCode, materialName, 
            category, quantity, unitCost, reason);
    }

    public void AttachBeforePhoto(Guid photoId)
    {
        BeforePhotoId = photoId;
    }

    public void AttachAfterPhoto(Guid photoId)
    {
        AfterPhotoId = photoId;
    }

    public void MarkAsSubmitted()
    {
        Status = MaterialUsageStatus.Submitted;
    }

    public void Approve()
    {
        Status = MaterialUsageStatus.Approved;
    }

    public void Reject()
    {
        Status = MaterialUsageStatus.Rejected;
    }

    public bool HasRequiredPhotos()
    {
        return BeforePhotoId.HasValue && AfterPhotoId.HasValue;
    }
}
