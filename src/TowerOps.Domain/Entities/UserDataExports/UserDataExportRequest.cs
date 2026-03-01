using TowerOps.Domain.Common;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;

namespace TowerOps.Domain.Entities.UserDataExports;

public sealed class UserDataExportRequest : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public UserDataExportStatus Status { get; private set; }
    public string? PayloadJson { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? FailureReason { get; private set; }

    private UserDataExportRequest() : base()
    {
    }

    private UserDataExportRequest(Guid userId, DateTime requestedAtUtc, DateTime expiresAtUtc)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        RequestedAtUtc = requestedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        Status = UserDataExportStatus.Pending;
    }

    public static UserDataExportRequest Create(Guid userId, int expiryDays)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User id is required.", "UserDataExportRequest.UserId.Required");

        if (expiryDays <= 0)
            throw new DomainException("Export expiry days must be positive.", "UserDataExportRequest.ExpiryDays.Positive");

        var now = DateTime.UtcNow;
        return new UserDataExportRequest(userId, now, now.AddDays(expiryDays));
    }

    public void Complete(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            throw new DomainException("Export payload is required.", "UserDataExportRequest.Payload.Required");

        PayloadJson = payloadJson;
        Status = UserDataExportStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        FailureReason = null;
    }

    public void Fail(string reason)
    {
        FailureReason = string.IsNullOrWhiteSpace(reason) ? "Failed to generate export." : reason.Trim();
        Status = UserDataExportStatus.Failed;
        CompletedAtUtc = null;
    }

    public void MarkExpired(DateTime nowUtc)
    {
        if (nowUtc >= ExpiresAtUtc && Status == UserDataExportStatus.Completed)
        {
            Status = UserDataExportStatus.Expired;
        }
    }

    public bool IsAvailable(DateTime nowUtc)
    {
        return Status == UserDataExportStatus.Completed && nowUtc <= ExpiresAtUtc;
    }
}
