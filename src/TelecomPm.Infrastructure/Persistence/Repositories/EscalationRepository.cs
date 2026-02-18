namespace TelecomPM.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.Escalations;
using TelecomPM.Domain.Interfaces.Repositories;

public class EscalationRepository : Repository<Escalation, Guid>, IEscalationRepository
{
    public EscalationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Escalation?> GetByIncidentIdAsync(string incidentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.IncidentId == incidentId, cancellationToken);
    }
}
