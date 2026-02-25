using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.MaterialSpecifications;

public sealed class MaterialsByCategorySpecification : BaseSpecification<Material>
{
    public MaterialsByCategorySpecification(MaterialCategory category, Guid? officeId = null)
        : base(m => m.Category == category && 
                    (!officeId.HasValue || m.OfficeId == officeId.Value) && 
                    !m.IsDeleted)
    {
        ApplyOrderBy(m => m.Name);
    }
}
