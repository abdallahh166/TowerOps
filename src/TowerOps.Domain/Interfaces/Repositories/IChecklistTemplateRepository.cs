using TowerOps.Domain.Entities.ChecklistTemplates;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IChecklistTemplateRepository : IRepository<ChecklistTemplate, Guid>
{
    Task<ChecklistTemplate?> GetActiveByVisitTypeAsync(VisitType visitType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChecklistTemplate>> GetByVisitTypeAsync(VisitType visitType, CancellationToken cancellationToken = default);
}
