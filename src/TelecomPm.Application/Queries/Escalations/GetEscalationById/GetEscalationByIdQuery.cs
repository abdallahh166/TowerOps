namespace TelecomPM.Application.Queries.Escalations.GetEscalationById;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Escalations;

public record GetEscalationByIdQuery : IQuery<EscalationDto>
{
    public Guid EscalationId { get; init; }
}
