using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

namespace TelecomPM.Application.Commands.Sites.ImportSiteData;

public record ImportSiteDataCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}
