using TowerOps.Domain.Common;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Entities.Visits
{
    public class VisitPhoto : Entity<Guid>
    {
        public Guid VisitId { get; private set; }
        public PhotoType Type { get; private set; }
        public PhotoCategory Category { get; private set; }
        public string ItemName { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public UploadedFileStatus FileStatus { get; private set; }
        public DateTime? ScanCompletedAtUtc { get; private set; }
        public string? QuarantineReason { get; private set; }
        public string? ThumbnailPath { get; private set; }
        public string? Description { get; private set; }
        public DateTime? CapturedAtUtc { get; private set; }
        public Location Location { get; private set; } = Location.Empty;
        public Visit? Visit { get; private set; }

        private VisitPhoto() { } // For EF Core

        private VisitPhoto(
            Guid visitId,
            PhotoType type,
            PhotoCategory category,
            string itemName,
            string fileName,
            string filePath,
            UploadedFileStatus fileStatus,
            string? thumbnailPath = null,
            string? description = null,
            Location? location = null)
        {
            VisitId = visitId;
            Type = type;
            Category = category;
            ItemName = itemName;
            FileName = fileName;
            FilePath = filePath;
            FileStatus = fileStatus;
            ThumbnailPath = thumbnailPath;
            Description = description;
            CapturedAtUtc = DateTime.UtcNow;
            Location = location ?? Location.Empty;
        }

        // ✅ Factory method
        public static VisitPhoto Create(
            Guid visitId,
            PhotoType type,
            PhotoCategory category,
            string itemName,
            string fileName,
            string filePath)
        {
            // Legacy/default creation path is marked approved for backward compatibility.
            return new VisitPhoto(visitId, type, category, itemName, fileName, filePath, UploadedFileStatus.Approved);
        }

        public static VisitPhoto CreatePendingUpload(
            Guid visitId,
            PhotoType type,
            PhotoCategory category,
            string itemName,
            string fileName,
            string filePath)
        {
            return new VisitPhoto(visitId, type, category, itemName, fileName, filePath, UploadedFileStatus.Pending);
        }

        // ✅ Setters
        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetLocation(Coordinates coordinates)
        {
            Location = new Location(coordinates.Latitude, coordinates.Longitude);
        }

        public void SetCapturedAtUtc(DateTime capturedAtUtc)
        {
            CapturedAtUtc = capturedAtUtc.Kind switch
            {
                DateTimeKind.Utc => capturedAtUtc,
                DateTimeKind.Local => capturedAtUtc.ToUniversalTime(),
                _ => DateTime.SpecifyKind(capturedAtUtc, DateTimeKind.Utc)
            };
        }

        // ✅ Validation Helper
        public bool MeetsRequirements()
        {
            // ممكن بعدين تتحقق من حاجات زي أبعاد الصورة أو الحجم لو عايز
            return !string.IsNullOrWhiteSpace(FilePath)
                   && !string.IsNullOrWhiteSpace(FileName);
        }

        public void UpdateDescription(string? newDescription, string updatedBy)
        {
            Description = newDescription;
            MarkAsUpdated(updatedBy);
        }

        public void UpdateFilePath(string newPath, string updatedBy)
        {
            FilePath = newPath;
            MarkAsUpdated(updatedBy);
        }

        public void MarkApproved(string updatedBy)
        {
            FileStatus = UploadedFileStatus.Approved;
            ScanCompletedAtUtc = DateTime.UtcNow;
            QuarantineReason = null;
            MarkAsUpdated(updatedBy);
        }

        public void MarkQuarantined(string reason, string updatedBy)
        {
            FileStatus = UploadedFileStatus.Quarantined;
            ScanCompletedAtUtc = DateTime.UtcNow;
            QuarantineReason = reason;
            MarkAsUpdated(updatedBy);
        }

        public void DeletePhoto(string deletedBy)
        {
            MarkAsDeleted(deletedBy);
        }
    }

    public class Location
    {
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        private Location() { }

        public Location(double? latitude, double? longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Location Empty => new Location(null, null);
    }
}
