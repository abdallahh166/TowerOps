using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.MaterialSpecifications;

public sealed class MaterialsBySupplierSpecification : BaseSpecification<Material>
{
    public MaterialsBySupplierSpecification(string supplier, Guid? officeId = null)
        : base(m => m.Supplier != null && 
                   m.Supplier.ToUpper() == supplier.ToUpper() &&
                   (!officeId.HasValue || m.OfficeId == officeId.Value) &&
                   m.IsActive)
    {
        AddInclude(m => m.Transactions);
        AddOrderBy(m => m.Name);
    }
}

