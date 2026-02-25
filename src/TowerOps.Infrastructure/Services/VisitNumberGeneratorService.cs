using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Services;

namespace TowerOps.Infrastructure.Services;

public sealed class VisitNumberGeneratorService : IVisitNumberGeneratorService
{
    private readonly IVisitRepository _visitRepository;

    public VisitNumberGeneratorService(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public async Task<string> GenerateNextVisitNumberAsync(CancellationToken cancellationToken = default)
    {
        // Format: V{YEAR}{SEQUENCE}
        // Example: V2025001, V2025002, etc.
        
        var year = DateTime.UtcNow.Year;
        var lastVisitNumber = await _visitRepository.GenerateVisitNumberAsync(cancellationToken);
        
        int sequence = 1;
        if (!string.IsNullOrEmpty(lastVisitNumber))
        {
            var lastSequence = int.Parse(lastVisitNumber.Substring(5));
            sequence = lastSequence + 1;
        }

        return $"V{year}{sequence:D6}";
    }
}
