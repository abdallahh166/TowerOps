using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.MaterialSpecifications;

public sealed class LowStockMaterialsSpecification : BaseSpecification<Material>
{
    public LowStockMaterialsSpecification(Guid officeId)
        : base(m => m.OfficeId == officeId && 
                    m.CurrentStock <= m.MinimumStock && 
                    !m.IsDeleted)
    {
        ApplyOrderBy(m => m.CurrentStock);
    }
}
