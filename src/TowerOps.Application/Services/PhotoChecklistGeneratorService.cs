using System.Collections.Generic;
using System.Linq;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Services;
using TowerOps.Domain.Models;


namespace TowerOps.Application.Services;

public sealed class PhotoChecklistGeneratorService : IPhotoChecklistGeneratorService
{
    public List<PhotoChecklistItem> GenerateChecklistForSite(Site site)
    {
        var checklist = new List<PhotoChecklistItem>();

        // Base photos (always required)
        AddBasePhotos(checklist);

        // Tower photos
        if (site.TowerInfo != null)
        {
            AddTowerPhotos(checklist, site.TowerInfo);
        }

        // Power system photos
        if (site.PowerSystem != null)
        {
            AddPowerSystemPhotos(checklist, site.PowerSystem);
        }

        // Cooling system photos
        if (site.CoolingSystem != null)
        {
            AddCoolingSystemPhotos(checklist, site.CoolingSystem);
        }

        // Radio equipment photos
        if (site.RadioEquipment != null)
        {
            AddRadioEquipmentPhotos(checklist, site.RadioEquipment);
        }

        // Fire safety photos
        if (site.FireSafety != null)
        {
            AddFireSafetyPhotos(checklist);
        }

        // Sharing photos
        if (site.SharingInfo?.IsShared == true)
        {
            AddSharingPhotos(checklist);
        }

        return checklist;
    }

    private void AddBasePhotos(List<PhotoChecklistItem> checklist)
    {
        checklist.Add(new PhotoChecklistItem("Shelter Inside - Side 1", PhotoCategory.ShelterInside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Inside - Side 2", PhotoCategory.ShelterInside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Inside - Side 3", PhotoCategory.ShelterInside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Inside - Side 4", PhotoCategory.ShelterInside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Outside - Side 1", PhotoCategory.ShelterOutside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Outside - Side 2", PhotoCategory.ShelterOutside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Outside - Side 3", PhotoCategory.ShelterOutside, true));
        checklist.Add(new PhotoChecklistItem("Shelter Outside - Side 4", PhotoCategory.ShelterOutside, true));
        checklist.Add(new PhotoChecklistItem("Main Earth Bar", PhotoCategory.EarthBar, true));
        checklist.Add(new PhotoChecklistItem("Earth Rod", PhotoCategory.EarthRod, true));
    }

    private void AddTowerPhotos(List<PhotoChecklistItem> checklist, SiteTowerInfo towerInfo)
    {
        checklist.Add(new PhotoChecklistItem("Tower - Side 1", PhotoCategory.Tower, true));
        checklist.Add(new PhotoChecklistItem("Tower - Side 2", PhotoCategory.Tower, true));
        checklist.Add(new PhotoChecklistItem("Tower - Side 3", PhotoCategory.Tower, true));
        checklist.Add(new PhotoChecklistItem("Tower - Side 4", PhotoCategory.Tower, true));
        checklist.Add(new PhotoChecklistItem("Tower from Distance", PhotoCategory.Tower, true));
    }

    private void AddPowerSystemPhotos(List<PhotoChecklistItem> checklist, SitePowerSystem powerSystem)
    {
        checklist.Add(new PhotoChecklistItem("GEDP Inside", PhotoCategory.GEDP, true));
        checklist.Add(new PhotoChecklistItem("GEDP Outside", PhotoCategory.GEDP, true));
        checklist.Add(new PhotoChecklistItem("GEDP Circuit Diagram", PhotoCategory.GEDP, true));
        checklist.Add(new PhotoChecklistItem("Rectifier Inside", PhotoCategory.Rectifier, true));
        checklist.Add(new PhotoChecklistItem("Rectifier Modules", PhotoCategory.Rectifier, true));
        checklist.Add(new PhotoChecklistItem("Batteries Overview", PhotoCategory.Batteries, true));
        checklist.Add(new PhotoChecklistItem("Batteries Rack Earth", PhotoCategory.Batteries, true));
        checklist.Add(new PhotoChecklistItem("Power Meter Reading", PhotoCategory.PowerMeter, true));

        if (powerSystem.HasGenerator)
        {
            checklist.Add(new PhotoChecklistItem("Generator Control Panel", PhotoCategory.Generator, true));
            checklist.Add(new PhotoChecklistItem("Generator Engine", PhotoCategory.Generator, true));
        }

        if (powerSystem.HasSolarPanel)
        {
            checklist.Add(new PhotoChecklistItem("Solar Panels Array", PhotoCategory.Other, false));
            checklist.Add(new PhotoChecklistItem("Solar Inverter", PhotoCategory.Other, false));
        }
    }

    private void AddCoolingSystemPhotos(List<PhotoChecklistItem> checklist, SiteCoolingSystem coolingSystem)
    {
        for (int i = 1; i <= coolingSystem.ACUnitsCount; i++)
        {
            checklist.Add(new PhotoChecklistItem($"A/C {i} IDU", PhotoCategory.AirConditioner, true));
            checklist.Add(new PhotoChecklistItem($"A/C {i} ODU", PhotoCategory.AirConditioner, true));
            checklist.Add(new PhotoChecklistItem($"A/C {i} Terminals", PhotoCategory.AirConditioner, true));
        }
    }

    private void AddRadioEquipmentPhotos(List<PhotoChecklistItem> checklist, SiteRadioEquipment radioEquipment)
    {
        if (radioEquipment.Has2G)
        {
            checklist.Add(new PhotoChecklistItem("2G BTS Cabinet", PhotoCategory.BTS, true));
            checklist.Add(new PhotoChecklistItem("2G DDF Inside", PhotoCategory.DDF, true));
        }

        if (radioEquipment.Has3G)
        {
            checklist.Add(new PhotoChecklistItem("3G NodeB Cabinet", PhotoCategory.NodeB, true));
            checklist.Add(new PhotoChecklistItem("3G DDF Inside", PhotoCategory.DDF, true));
        }

        if (radioEquipment.Has4G)
        {
            checklist.Add(new PhotoChecklistItem("4G Equipment", PhotoCategory.Other, true));
        }
    }

    private void AddFireSafetyPhotos(List<PhotoChecklistItem> checklist)
    {
        checklist.Add(new PhotoChecklistItem("Fire Panel Inside", PhotoCategory.FirePanel, true));
        checklist.Add(new PhotoChecklistItem("Fire Panel Outside", PhotoCategory.FirePanel, true));
        checklist.Add(new PhotoChecklistItem("Fire Extinguisher", PhotoCategory.FireExtinguisher, true));
    }

    private void AddSharingPhotos(List<PhotoChecklistItem> checklist)
    {
        checklist.Add(new PhotoChecklistItem("Sharing Panel", PhotoCategory.Other, true));
        checklist.Add(new PhotoChecklistItem("Power Sharing Meter", PhotoCategory.PowerMeter, true));
    }
}
