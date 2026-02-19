namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.WorkOrders;
using TelecomPM.Application.Commands.WorkOrders.AssignWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;
using TelecomPM.Application.Queries.WorkOrders.GetWorkOrderById;

public static class WorkOrdersContractMapper
{
    public static CreateWorkOrderCommand ToCommand(this CreateWorkOrderRequest request)
        => new()
        {
            WoNumber = request.WoNumber,
            SiteCode = request.SiteCode,
            OfficeCode = request.OfficeCode,
            SlaClass = request.SlaClass,
            IssueDescription = request.IssueDescription
        };

    public static GetWorkOrderByIdQuery ToWorkOrderByIdQuery(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };

    public static AssignWorkOrderCommand ToCommand(this AssignWorkOrderRequest request, Guid workOrderId)
        => new()
        {
            WorkOrderId = workOrderId,
            EngineerId = request.EngineerId,
            EngineerName = request.EngineerName,
            AssignedBy = request.AssignedBy
        };
}
