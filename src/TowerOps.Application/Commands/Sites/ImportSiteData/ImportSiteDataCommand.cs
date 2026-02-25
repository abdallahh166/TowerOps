using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

namespace TowerOps.Application.Commands.Sites.ImportSiteData;

public record ImportSiteDataCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}
