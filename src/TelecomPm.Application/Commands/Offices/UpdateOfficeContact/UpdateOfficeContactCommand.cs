namespace TelecomPM.Application.Commands.Offices.UpdateOfficeContact;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;

public record UpdateOfficeContactCommand : ICommand<OfficeDto>
{
    public Guid OfficeId { get; init; }
    public string ContactPerson { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
}

