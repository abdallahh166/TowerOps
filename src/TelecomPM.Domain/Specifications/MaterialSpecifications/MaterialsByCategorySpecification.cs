using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.MaterialSpecifications;

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
