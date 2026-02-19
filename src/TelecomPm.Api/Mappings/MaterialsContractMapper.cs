namespace TelecomPm.Api.Mappings;

using TelecomPM.Application.Queries.Materials.GetLowStockMaterials;

public static class MaterialsContractMapper
{
    public static GetLowStockMaterialsQuery ToLowStockQuery(this Guid officeId)
        => new() { OfficeId = officeId };
}
