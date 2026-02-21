namespace TelecomPM.Application.Security;

public static class PermissionConstants
{
    public const string ClaimType = "permission";

    // Site
    public const string SitesView = "sites.view";
    public const string SitesCreate = "sites.create";
    public const string SitesEdit = "sites.edit";
    public const string SitesImport = "sites.import";
    public const string SitesExport = "sites.export";

    // Visit
    public const string VisitsView = "visits.view";
    public const string VisitsCreate = "visits.create";
    public const string VisitsStart = "visits.start";
    public const string VisitsSubmit = "visits.submit";
    public const string VisitsApprove = "visits.approve";
    public const string VisitsReview = "visits.review";
    public const string VisitsCancel = "visits.cancel";

    // Work orders
    public const string WorkOrdersView = "workorders.view";
    public const string WorkOrdersCreate = "workorders.create";
    public const string WorkOrdersAssign = "workorders.assign";
    public const string WorkOrdersComplete = "workorders.complete";
    public const string WorkOrdersClose = "workorders.close";

    // BDT
    public const string BdtView = "bdt.view";
    public const string BdtCreate = "bdt.create";
    public const string BdtImport = "bdt.import";

    // Reports
    public const string ReportsView = "reports.view";
    public const string ReportsExport = "reports.export";
    public const string ReportsScorecard = "reports.scorecard";

    // KPI
    public const string KpiView = "kpi.view";
    public const string KpiAnalytics = "kpi.analytics";

    // Users
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersEdit = "users.edit";
    public const string UsersDelete = "users.delete";
    public const string UsersChangeRole = "users.change_role";

    // Settings
    public const string SettingsView = "settings.view";
    public const string SettingsEdit = "settings.edit";

    // Materials
    public const string MaterialsView = "materials.view";
    public const string MaterialsManage = "materials.manage";

    // Escalations
    public const string EscalationsView = "escalations.view";
    public const string EscalationsCreate = "escalations.create";
    public const string EscalationsApprove = "escalations.approve";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SitesView, SitesCreate, SitesEdit, SitesImport, SitesExport,
        VisitsView, VisitsCreate, VisitsStart, VisitsSubmit, VisitsApprove, VisitsReview, VisitsCancel,
        WorkOrdersView, WorkOrdersCreate, WorkOrdersAssign, WorkOrdersComplete, WorkOrdersClose,
        BdtView, BdtCreate, BdtImport,
        ReportsView, ReportsExport, ReportsScorecard,
        KpiView, KpiAnalytics,
        UsersView, UsersCreate, UsersEdit, UsersDelete, UsersChangeRole,
        SettingsView, SettingsEdit,
        MaterialsView, MaterialsManage,
        EscalationsView, EscalationsCreate, EscalationsApprove
    };
}
