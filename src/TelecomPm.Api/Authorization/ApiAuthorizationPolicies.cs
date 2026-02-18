namespace TelecomPM.Api.Authorization;

using Microsoft.AspNetCore.Authorization;
using TelecomPM.Domain.Enums;

public static class ApiAuthorizationPolicies
{
    public const string CanManageWorkOrders = "CanManageWorkOrders";
    public const string CanViewWorkOrders = "CanViewWorkOrders";
    public const string CanReviewVisits = "CanReviewVisits";
    public const string CanManageEscalations = "CanManageEscalations";
    public const string CanViewEscalations = "CanViewEscalations";
    public const string CanViewKpis = "CanViewKpis";

    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(CanManageWorkOrders, policy =>
            policy.RequireRole(
                UserRole.Admin.ToString(),
                UserRole.Manager.ToString(),
                UserRole.Supervisor.ToString()));

        options.AddPolicy(CanViewWorkOrders, policy =>
            policy.RequireRole(
                UserRole.Admin.ToString(),
                UserRole.Manager.ToString(),
                UserRole.Supervisor.ToString(),
                UserRole.PMEngineer.ToString()));

        options.AddPolicy(CanReviewVisits, policy =>
            policy.RequireRole(
                UserRole.Admin.ToString(),
                UserRole.Manager.ToString(),
                UserRole.Supervisor.ToString()));

        options.AddPolicy(CanManageEscalations, policy =>
            policy.RequireRole(
                UserRole.Admin.ToString(),
                UserRole.Manager.ToString(),
                UserRole.Supervisor.ToString()));

        options.AddPolicy(CanViewEscalations, policy =>
            policy.RequireRole(
                UserRole.Admin.ToString(),
                UserRole.Manager.ToString(),
                UserRole.Supervisor.ToString(),
                UserRole.PMEngineer.ToString()));

        options.AddPolicy(CanViewKpis, policy =>
            policy.RequireRole(
                UserRole.Admin.ToString(),
                UserRole.Manager.ToString(),
                UserRole.Supervisor.ToString()));

    }
}
