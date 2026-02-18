namespace TelecomPm.Api.Contracts.Offices;

using System.ComponentModel.DataAnnotations;

public record UpdateOfficeContactRequest
{
    [StringLength(200)]
    public string? ContactPerson { get; init; }

    [Phone]
    [StringLength(50)]
    public string? ContactPhone { get; init; }

    [EmailAddress]
    [StringLength(200)]
    public string? ContactEmail { get; init; }
}

