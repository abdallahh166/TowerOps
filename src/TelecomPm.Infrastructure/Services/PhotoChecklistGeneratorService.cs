using System.Collections.Generic;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Models;
using TelecomPM.Domain.Services;

namespace TelecomPM.Infrastructure.Services;

public sealed class PhotoChecklistGeneratorService : IPhotoChecklistGeneratorService
{
    public List<PhotoChecklistItem> GenerateChecklistForSite(Site site)
    {
        var checklist = new List<PhotoChecklistItem>();

        // Mandatory for all sites
        checklist.AddRange(GetBasicPhotos());

        // Power system photos
        if (site.PowerSystem != null)
        {
            checklist.AddRange(GetPowerSystemPhotos(site.PowerSystem));
        }

        // Cooling system photos
        if (site.CoolingSystem != null)
        {
            checklist.AddRange(GetCoolingSystemPhotos(site.CoolingSystem));
        }

        // Radio equipment photos
        if (site.RadioEquipment != null)
        {
            checklist.AddRange(GetRadioEquipmentPhotos(site.RadioEquipment));
        }

        // Fire safety photos
        if (site.FireSafety != null)
        {
            checklist.Add(new PhotoChecklistItem(
                "Fire Panel",
                Domain.Enums.PhotoCategory.FirePanel,
                true));
            checklist.Add(new PhotoChecklistItem(
                "Fire Extinguishers",
                Domain.Enums.PhotoCategory.FireExtinguisher,
                true));
        }

        return checklist;
    }

    private List<PhotoChecklistItem> GetBasicPhotos()
    {
        return new List<PhotoChecklistItem>
        {
            new("Shelter Inside", Domain.Enums.PhotoCategory.ShelterInside, true),
            new("Shelter Outside", Domain.Enums.PhotoCategory.ShelterOutside, true),
            new("Tower", Domain.Enums.PhotoCategory.Tower, true),
            new("Fence", Domain.Enums.PhotoCategory.Fence, true),
            new("Earth Bar", Domain.Enums.PhotoCategory.EarthBar, true),
            new("Earth Rod", Domain.Enums.PhotoCategory.EarthRod, true),
            new("PM Logbook", Domain.Enums.PhotoCategory.PMLogbook, true)
        };
    }

    private List<PhotoChecklistItem> GetPowerSystemPhotos(SitePowerSystem powerSystem)
    {
        var photos = new List<PhotoChecklistItem>
        {
            new("GEDP", Domain.Enums.PhotoCategory.GEDP, true),
            new("Rectifier", Domain.Enums.PhotoCategory.Rectifier, true),
            new("Batteries", Domain.Enums.PhotoCategory.Batteries, true),
            new("Power Meter", Domain.Enums.PhotoCategory.PowerMeter, true),
            new("Surge Arrestor", Domain.Enums.PhotoCategory.SurgeArrestor, true)
        };

        if (powerSystem.HasGenerator)
        {
            photos.Add(new PhotoChecklistItem(
                "Generator",
                Domain.Enums.PhotoCategory.Generator,
                true));
        }

        return photos;
    }

    private List<PhotoChecklistItem> GetCoolingSystemPhotos(SiteCoolingSystem coolingSystem)
    {
        var photos = new List<PhotoChecklistItem>();

        for (int i = 1; i <= coolingSystem.ACUnitsCount; i++)
        {
            photos.Add(new PhotoChecklistItem(
                $"Air Conditioner {i}",
                Domain.Enums.PhotoCategory.AirConditioner,
                true));
        }

        return photos;
    }

    private List<PhotoChecklistItem> GetRadioEquipmentPhotos(SiteRadioEquipment radioEquipment)
    {
        var photos = new List<PhotoChecklistItem>();

        if (radioEquipment.Has2G)
        {
            photos.Add(new PhotoChecklistItem("BTS", Domain.Enums.PhotoCategory.BTS, true));
        }

        if (radioEquipment.Has3G || radioEquipment.Has4G)
        {
            photos.Add(new PhotoChecklistItem("NodeB", Domain.Enums.PhotoCategory.NodeB, true));
        }

        photos.Add(new PhotoChecklistItem("DDF", Domain.Enums.PhotoCategory.DDF, true));

        return photos;
    }
}

