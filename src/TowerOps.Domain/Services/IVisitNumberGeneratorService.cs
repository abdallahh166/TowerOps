namespace TowerOps.Domain.Services;

public interface IVisitNumberGeneratorService
{
    Task<string> GenerateNextVisitNumberAsync(CancellationToken cancellationToken = default);
}
