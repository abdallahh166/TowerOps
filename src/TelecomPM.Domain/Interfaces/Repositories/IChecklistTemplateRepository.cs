using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IChecklistTemplateRepository : IRepository<ChecklistTemplate, Guid>
{
    Task<ChecklistTemplate?> GetActiveByVisitTypeAsync(VisitType visitType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChecklistTemplate>> GetByVisitTypeAsync(VisitType visitType, CancellationToken cancellationToken = default);
}
