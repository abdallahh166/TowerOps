namespace TelecomPM.Api.Authorization;

using Microsoft.AspNetCore.Authorization;
using TelecomPM.Application.Security;

public static class ApiAuthorizationPolicies
{
    public const string CanManageWorkOrders = "CanManageWorkOrders";
    public const string CanViewWorkOrders = "CanViewWorkOrders";
    public const string CanReviewVisits = "CanReviewVisits";
    public const string CanManageEscalations = "CanManageEscalations";
    public const string CanViewEscalations = "CanViewEscalations";
    public const string CanViewKpis = "CanViewKpis";
    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageOffices = "CanManageOffices";
    public const string CanManageSites = "CanManageSites";
    public const string CanViewAnalytics = "CanViewAnalytics";
    public const string CanViewSites = "CanViewSites";
    public const string CanViewReports = "CanViewReports";
    public const string CanViewMaterials = "CanViewMaterials";
    public const string CanManageMaterials = "CanManageMaterials";
    public const string CanManageSettings = "CanManageSettings";

    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(CanManageWorkOrders, policy =>
            RequireAnyPermission(policy,
                PermissionConstants.WorkOrdersCreate,
                PermissionConstants.WorkOrdersAssign,
                PermissionConstants.WorkOrdersComplete,
                PermissionConstants.WorkOrdersClose));

        options.AddPolicy(CanViewWorkOrders, policy =>
            RequireAnyPermission(policy, PermissionConstants.WorkOrdersView));

        options.AddPolicy(CanReviewVisits, policy =>
            RequireAnyPermission(policy,
                PermissionConstants.VisitsReview,
                PermissionConstants.VisitsApprove));

        options.AddPolicy(CanManageEscalations, policy =>
            RequireAnyPermission(policy, PermissionConstants.EscalationsApprove));

        options.AddPolicy(CanViewEscalations, policy =>
            RequireAnyPermission(policy, PermissionConstants.EscalationsView));

        options.AddPolicy(CanViewKpis, policy =>
            RequireAnyPermission(policy, PermissionConstants.KpiView));

        options.AddPolicy(CanManageUsers, policy =>
            RequireAnyPermission(policy,
                PermissionConstants.UsersCreate,
                PermissionConstants.UsersEdit,
                PermissionConstants.UsersDelete,
                PermissionConstants.UsersChangeRole));

        options.AddPolicy(CanManageOffices, policy =>
            RequireAnyPermission(policy, PermissionConstants.SettingsEdit));

        options.AddPolicy(CanManageSites, policy =>
            RequireAnyPermission(policy, PermissionConstants.SitesEdit));

        options.AddPolicy(CanViewAnalytics, policy =>
            RequireAnyPermission(policy, PermissionConstants.KpiAnalytics));

        options.AddPolicy(CanViewSites, policy =>
            RequireAnyPermission(policy, PermissionConstants.SitesView));

        options.AddPolicy(CanViewReports, policy =>
            RequireAnyPermission(policy, PermissionConstants.ReportsView));

        options.AddPolicy(CanViewMaterials, policy =>
            RequireAnyPermission(policy, PermissionConstants.MaterialsView));

        options.AddPolicy(CanManageMaterials, policy =>
            RequireAnyPermission(policy, PermissionConstants.MaterialsManage));

        options.AddPolicy(CanManageSettings, policy =>
            RequireAnyPermission(policy, PermissionConstants.SettingsEdit));
    }

    private static void RequireAnyPermission(AuthorizationPolicyBuilder policy, params string[] requiredPermissions)
    {
        policy.RequireAssertion(context =>
            context.User.Claims.Any(c =>
                c.Type == PermissionConstants.ClaimType &&
                requiredPermissions.Any(p => string.Equals(p, c.Value, StringComparison.OrdinalIgnoreCase))));
    }
}
