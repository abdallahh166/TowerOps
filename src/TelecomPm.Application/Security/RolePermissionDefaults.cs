using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Security;

public static class RolePermissionDefaults
{
    public static IReadOnlyList<string> GetDefaultPermissions(string roleName)
    {
        if (string.Equals(roleName, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
            return PermissionConstants.All;

        if (string.Equals(roleName, UserRole.Manager.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                PermissionConstants.SitesView,
                PermissionConstants.VisitsView,
                PermissionConstants.VisitsApprove,
                PermissionConstants.VisitsReview,
                PermissionConstants.WorkOrdersView,
                PermissionConstants.WorkOrdersCreate,
                PermissionConstants.WorkOrdersAssign,
                PermissionConstants.WorkOrdersClose,
                PermissionConstants.BdtView,
                PermissionConstants.ReportsView,
                PermissionConstants.ReportsExport,
                PermissionConstants.ReportsScorecard,
                PermissionConstants.KpiView,
                PermissionConstants.KpiAnalytics,
                PermissionConstants.UsersView,
                PermissionConstants.EscalationsView,
                PermissionConstants.EscalationsApprove,
                PermissionConstants.MaterialsView,
                PermissionConstants.SitesImport,
                PermissionConstants.SitesExport
            };
        }

        if (string.Equals(roleName, UserRole.Supervisor.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                PermissionConstants.SitesView,
                PermissionConstants.VisitsView,
                PermissionConstants.VisitsApprove,
                PermissionConstants.VisitsReview,
                PermissionConstants.WorkOrdersView,
                PermissionConstants.ReportsView,
                PermissionConstants.KpiView,
                PermissionConstants.EscalationsView,
                PermissionConstants.MaterialsView
            };
        }

        if (string.Equals(roleName, UserRole.PMEngineer.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                PermissionConstants.SitesView,
                PermissionConstants.VisitsView,
                PermissionConstants.VisitsCreate,
                PermissionConstants.VisitsStart,
                PermissionConstants.VisitsSubmit,
                PermissionConstants.WorkOrdersView,
                PermissionConstants.BdtView,
                PermissionConstants.BdtCreate,
                PermissionConstants.MaterialsView,
                PermissionConstants.EscalationsCreate
            };
        }

        if (string.Equals(roleName, UserRole.Technician.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return new[]
            {
                PermissionConstants.SitesView,
                PermissionConstants.VisitsView,
                PermissionConstants.WorkOrdersView,
                PermissionConstants.MaterialsView
            };
        }

        // For custom roles or unknown roles, start with no permissions.
        return Array.Empty<string>();
    }
}
