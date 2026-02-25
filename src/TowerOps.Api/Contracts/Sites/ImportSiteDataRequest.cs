using Microsoft.AspNetCore.Http;

namespace TowerOps.Api.Contracts.Sites;

public sealed class ImportSiteDataRequest
{
    public IFormFile File { get; set; } = null!;
}
