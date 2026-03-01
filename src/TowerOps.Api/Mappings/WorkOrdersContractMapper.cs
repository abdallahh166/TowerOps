namespace TowerOps.Api.Mappings;

using TowerOps.Api.Contracts.WorkOrders;
using TowerOps.Application.Commands.WorkOrders.AcceptByCustomer;
using TowerOps.Application.Commands.WorkOrders.AssignWorkOrder;
using TowerOps.Application.Commands.WorkOrders.CancelWorkOrder;
using TowerOps.Application.Commands.WorkOrders.CloseWorkOrder;
using TowerOps.Application.Commands.WorkOrders.CompleteWorkOrder;
using TowerOps.Application.Commands.WorkOrders.CreateWorkOrder;
using TowerOps.Application.Commands.WorkOrders.RejectByCustomer;
using TowerOps.Application.Commands.Signatures.CaptureWorkOrderSignature;
using TowerOps.Application.Commands.WorkOrders.StartWorkOrder;
using TowerOps.Application.Commands.WorkOrders.SubmitForCustomerAcceptance;
using TowerOps.Application.Queries.Signatures.GetWorkOrderSignatures;
using TowerOps.Application.Queries.WorkOrders.GetWorkOrderById;

public static class WorkOrdersContractMapper
{
    public static CreateWorkOrderCommand ToCommand(this CreateWorkOrderRequest request)
        => new()
        {
            WoNumber = request.WoNumber,
            SiteCode = request.SiteCode,
            OfficeCode = request.OfficeCode,
            WorkOrderType = request.WorkOrderType,
            ScheduledVisitDateUtc = request.ScheduledVisitDateUtc,
            SlaClass = request.SlaClass,
            Scope = request.Scope,
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

    public static CaptureWorkOrderSignatureCommand ToCommand(this CaptureWorkOrderSignatureRequest request, Guid workOrderId)
        => new()
        {
            WorkOrderId = workOrderId,
            SignerName = request.SignerName,
            SignerRole = request.SignerRole,
            SignatureDataBase64 = request.SignatureDataBase64,
            SignerPhone = request.SignerPhone,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsEngineerSignature = request.IsEngineerSignature
        };

    public static GetWorkOrderSignaturesQuery ToWorkOrderSignaturesQuery(this Guid workOrderId)
        => new() { WorkOrderId = workOrderId };
}
