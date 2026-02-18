namespace TelecomPM.Application.Services;

using TelecomPM.Domain.Enums;

public interface IEscalationRoutingService
{
    EscalationLevel DetermineLevel(SlaClass slaClass, decimal financialImpactEgp, decimal slaImpactPercentage);
}
