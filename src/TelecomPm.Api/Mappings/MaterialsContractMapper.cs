namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Materials;
using TelecomPM.Application.Commands.Materials.ConsumeMaterial;
using TelecomPM.Application.Commands.Materials.CreateMaterial;
using TelecomPM.Application.Commands.Materials.DeleteMaterial;
using TelecomPM.Application.Commands.Materials.ReserveMaterial;
using TelecomPM.Application.Commands.Materials.RestockMaterial;
using TelecomPM.Application.Commands.Materials.UpdateMaterial;
using TelecomPM.Application.Queries.Materials.GetMaterialById;
using TelecomPM.Application.Queries.Materials.GetMaterialsByOffice;
using TelecomPM.Application.Queries.Materials.GetLowStockMaterials;

public static class MaterialsContractMapper
{
    public static GetLowStockMaterialsQuery ToLowStockQuery(this Guid officeId)
        => new() { OfficeId = officeId };

    public static GetMaterialByIdQuery ToMaterialByIdQuery(this Guid materialId)
        => new() { MaterialId = materialId };

    public static GetMaterialsByOfficeQuery ToQuery(this Guid officeId, bool? onlyInStock = null)
        => new()
        {
            OfficeId = officeId,
            OnlyInStock = onlyInStock
        };

    public static CreateMaterialCommand ToCommand(this CreateMaterialRequest request)
        => new()
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Category = request.Category,
            OfficeId = request.OfficeId,
            InitialStock = request.InitialStock,
            Unit = request.Unit,
            MinimumStock = request.MinimumStock,
            UnitCost = request.UnitCost,
            Supplier = request.Supplier
        };

    public static UpdateMaterialCommand ToCommand(this UpdateMaterialRequest request, Guid materialId)
        => new()
        {
            MaterialId = materialId,
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Category = request.Category
        };

    public static DeleteMaterialCommand ToDeleteMaterialCommand(this Guid materialId, string deletedBy)
        => new()
        {
            MaterialId = materialId,
            DeletedBy = deletedBy
        };

    public static RestockMaterialCommand ToCommand(this AddStockRequest request, Guid materialId, string restockedBy)
        => new()
        {
            MaterialId = materialId,
            Quantity = request.Quantity,
            Unit = request.Unit,
            RestockedBy = restockedBy,
            Supplier = request.Supplier
        };

    public static ReserveMaterialCommand ToCommand(this ReserveStockRequest request, Guid materialId)
        => new()
        {
            MaterialId = materialId,
            VisitId = request.VisitId,
            Quantity = request.Quantity,
            Unit = request.Unit
        };

    public static ConsumeMaterialCommand ToCommand(this ConsumeStockRequest request, Guid materialId, string performedBy)
        => new()
        {
            MaterialId = materialId,
            VisitId = request.VisitId,
            PerformedBy = performedBy
        };
}
