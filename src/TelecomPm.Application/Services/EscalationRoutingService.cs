namespace TelecomPM.Application.Services;

using TelecomPM.Domain.Enums;

public sealed class EscalationRoutingService : IEscalationRoutingService
{
    public EscalationLevel DetermineLevel(SlaClass slaClass, decimal financialImpactEgp, decimal slaImpactPercentage)
    {
        if (financialImpactEgp >= 250_000m)
        {
            return EscalationLevel.ProjectSponsor;
        }

        if (financialImpactEgp >= 50_000m || slaImpactPercentage >= 20m)
        {
            return EscalationLevel.BMManagement;
        }

        if (slaClass == SlaClass.P1)
        {
            return EscalationLevel.AreaManager;
        }

        return EscalationLevel.AreaManager;
    }
}
