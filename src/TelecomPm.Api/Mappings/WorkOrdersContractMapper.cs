namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.WorkOrders;
using TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;
using TelecomPM.Application.Commands.WorkOrders.AssignWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.CancelWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.CloseWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.CompleteWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.RejectByCustomer;
using TelecomPM.Application.Commands.WorkOrders.StartWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.SubmitForCustomerAcceptance;
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

    public static StartWorkOrderCommand ToStartCommand(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };

    public static CompleteWorkOrderCommand ToCompleteCommand(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };

    public static CloseWorkOrderCommand ToCloseCommand(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };

    public static CancelWorkOrderCommand ToCancelCommand(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };

    public static SubmitForCustomerAcceptanceCommand ToSubmitForCustomerAcceptanceCommand(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };

    public static AcceptByCustomerCommand ToAcceptByCustomerCommand(this CustomerAcceptWorkOrderRequest request, Guid workOrderId)
        => new() { WorkOrderId = workOrderId, AcceptedBy = request.AcceptedBy };

    public static RejectByCustomerCommand ToRejectByCustomerCommand(this CustomerRejectWorkOrderRequest request, Guid workOrderId)
        => new() { WorkOrderId = workOrderId, Reason = request.Reason };
}
