namespace TelecomPM.Domain.Services;

public interface IVisitNumberGeneratorService
{
    Task<string> GenerateNextVisitNumberAsync(CancellationToken cancellationToken = default);
}
