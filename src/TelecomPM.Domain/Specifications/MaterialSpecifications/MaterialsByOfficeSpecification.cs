using TelecomPM.Domain.Entities.Materials;

namespace TelecomPM.Domain.Specifications.MaterialSpecifications;

public class MaterialsByOfficeSpecification : BaseSpecification<Material>
{
    public MaterialsByOfficeSpecification(Guid officeId)
        : base(m => m.OfficeId == officeId)
    {
        AddInclude(m => m.Transactions);
        AddOrderBy(m => m.Category);
        AddOrderBy(m => m.Name);
    }

    public MaterialsByOfficeSpecification(Guid officeId, bool onlyInStock)
        : base(m => m.OfficeId == officeId &&
                   (!onlyInStock || m.CurrentStock.HasStock))
    {
        AddInclude(m => m.Transactions);
        AddOrderBy(m => m.Name);
    }
}