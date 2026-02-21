using Microsoft.AspNetCore.Http;

namespace TelecomPm.Api.Contracts.Visits;

public sealed class ImportVisitEvidenceRequest
{
    public IFormFile File { get; set; } = null!;
}
