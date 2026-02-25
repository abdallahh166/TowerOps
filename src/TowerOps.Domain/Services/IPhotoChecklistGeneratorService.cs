using System.Collections.Generic;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Models;


namespace TowerOps.Domain.Services;

public interface IPhotoChecklistGeneratorService
{
    List<PhotoChecklistItem> GenerateChecklistForSite(Site site);
}
