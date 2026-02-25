using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.MaterialSpecifications;

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
