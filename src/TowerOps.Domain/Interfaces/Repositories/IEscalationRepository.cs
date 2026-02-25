using TowerOps.Domain.Entities.Escalations;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IEscalationRepository : IRepository<Escalation, Guid>
{
    Task<Escalation?> GetByIncidentIdAsync(string incidentId, CancellationToken cancellationToken = default);
}
