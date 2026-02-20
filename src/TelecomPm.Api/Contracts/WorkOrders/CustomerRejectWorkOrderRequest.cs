namespace TelecomPm.Api.Contracts.WorkOrders;

public sealed class CustomerRejectWorkOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}
