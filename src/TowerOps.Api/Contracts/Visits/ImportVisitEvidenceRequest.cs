using Microsoft.AspNetCore.Http;

namespace TowerOps.Api.Contracts.Visits;

public sealed class ImportVisitEvidenceRequest
{
    public IFormFile File { get; set; } = null!;
}
