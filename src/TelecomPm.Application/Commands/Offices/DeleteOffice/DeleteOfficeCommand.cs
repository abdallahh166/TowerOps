namespace TelecomPM.Application.Commands.Offices.DeleteOffice;

using System;
using TelecomPM.Application.Common;

public record DeleteOfficeCommand : ICommand
{
    public Guid OfficeId { get; init; }
    public string DeletedBy { get; init; } = string.Empty;
}

