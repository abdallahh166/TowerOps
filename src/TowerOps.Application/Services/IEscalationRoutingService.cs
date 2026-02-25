namespace TowerOps.Application.Services;

using TowerOps.Domain.Enums;

public interface IEscalationRoutingService
{
    EscalationLevel DetermineLevel(SlaClass slaClass, decimal financialImpactEgp, decimal slaImpactPercentage);
}
