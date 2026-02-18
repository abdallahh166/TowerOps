namespace TelecomPm.Api.Contracts.WorkOrders;

using TelecomPM.Domain.Enums;

public sealed class CreateWorkOrderRequest
{
    public string WoNumber { get; set; } = string.Empty;
    public string SiteCode { get; set; } = string.Empty;
    public string OfficeCode { get; set; } = string.Empty;
    public SlaClass SlaClass { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
}
