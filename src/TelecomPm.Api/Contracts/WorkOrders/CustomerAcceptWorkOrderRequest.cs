namespace TelecomPm.Api.Contracts.WorkOrders;

public sealed class CustomerAcceptWorkOrderRequest
{
    public string AcceptedBy { get; set; } = string.Empty;
}
