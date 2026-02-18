using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.Visits
{
    public class VisitPhoto : Entity<Guid>
    {
        public Guid VisitId { get; private set; }
        public PhotoType Type { get; private set; }
        public PhotoCategory Category { get; private set; }
        public string ItemName { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public string? ThumbnailPath { get; private set; }
        public string? Description { get; private set; }
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
            ThumbnailPath = thumbnailPath;
            Description = description;
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
            return new VisitPhoto(visitId, type, category, itemName, fileName, filePath);
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
