namespace TowerOps.Api.Contracts.WorkOrders;

public sealed class AssignWorkOrderRequest
{
    public Guid EngineerId { get; set; }
    public string EngineerName { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
}
