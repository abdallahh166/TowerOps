using Microsoft.AspNetCore.Http;

namespace TelecomPm.Api.Contracts.Sites;

public sealed class ImportSiteDataRequest
{
    public IFormFile File { get; set; } = null!;
}
