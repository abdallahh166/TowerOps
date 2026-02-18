using TelecomPM.Domain.Entities.Escalations;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IEscalationRepository : IRepository<Escalation, Guid>
{
    Task<Escalation?> GetByIncidentIdAsync(string incidentId, CancellationToken cancellationToken = default);
}
