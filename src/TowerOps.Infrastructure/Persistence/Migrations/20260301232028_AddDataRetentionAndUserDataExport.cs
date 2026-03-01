using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDataRetentionAndUserDataExport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "WorkOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "WorkOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "VisitReadings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitReadings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "VisitReadings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "VisitPhotos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitPhotos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "VisitPhotos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "VisitMaterialUsages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitMaterialUsages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "VisitMaterialUsages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "VisitIssues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitIssues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "VisitIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "VisitChecklists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitChecklists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "VisitChecklists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "VisitApprovals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitApprovals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "VisitApprovals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "UnusedAssets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "UnusedAssets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "UnusedAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SystemSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SyncQueues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SyncQueues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SyncQueues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SyncConflicts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SyncConflicts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SyncConflicts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SiteTransmissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteTransmissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SiteTransmissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SiteTowerInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteTowerInfos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SiteTowerInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SiteSharings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteSharings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SiteSharings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Sites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Sites",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SiteRadioEquipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteRadioEquipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SiteRadioEquipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SitePowerSystems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SitePowerSystems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SitePowerSystems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SiteFireSafeties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteFireSafeties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SiteFireSafeties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "SiteCoolingSystems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteCoolingSystems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "SiteCoolingSystems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "PasswordResetTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "PasswordResetTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "PasswordResetTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Offices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Offices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Offices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "MaterialTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "MaterialTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "MaterialTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Materials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Escalations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Escalations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Escalations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "DailyPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "DailyPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "DailyPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "ChecklistTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "ChecklistTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "ChecklistTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "BatteryDischargeTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "BatteryDischargeTests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "BatteryDischargeTests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "AuditLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "AuditLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "Assets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "ApprovalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "ApprovalRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "ApprovalRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderLegalHold",
                table: "ApplicationRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LegalHoldAppliedAtUtc",
                table: "ApplicationRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalHoldReason",
                table: "ApplicationRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserDataExportRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUnderLegalHold = table.Column<bool>(type: "bit", nullable: false),
                    LegalHoldAppliedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LegalHoldReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDataExportRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDataExportRequests_ExpiresAtUtc",
                table: "UserDataExportRequests",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserDataExportRequests_UserId",
                table: "UserDataExportRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDataExportRequests_UserId_Status",
                table: "UserDataExportRequests",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDataExportRequests");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "VisitReadings");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitReadings");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "VisitReadings");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "VisitPhotos");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitPhotos");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "VisitPhotos");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "VisitMaterialUsages");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitMaterialUsages");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "VisitMaterialUsages");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "VisitIssues");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitIssues");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "VisitIssues");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "VisitChecklists");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitChecklists");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "VisitChecklists");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "VisitApprovals");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "VisitApprovals");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "VisitApprovals");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "UnusedAssets");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "UnusedAssets");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "UnusedAssets");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SyncQueues");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SyncQueues");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SyncQueues");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SyncConflicts");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SyncConflicts");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SyncConflicts");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SiteTransmissions");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteTransmissions");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SiteTransmissions");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SiteTowerInfos");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteTowerInfos");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SiteTowerInfos");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SiteSharings");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteSharings");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SiteSharings");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SiteRadioEquipments");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteRadioEquipments");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SiteRadioEquipments");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SiteFireSafeties");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteFireSafeties");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SiteFireSafeties");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "SiteCoolingSystems");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "SiteCoolingSystems");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "SiteCoolingSystems");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "MaterialTransactions");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "MaterialTransactions");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "MaterialTransactions");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Escalations");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Escalations");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Escalations");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "ChecklistTemplates");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "ChecklistTemplates");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "ChecklistTemplates");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "BatteryDischargeTests");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "BatteryDischargeTests");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "BatteryDischargeTests");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "ApprovalRecords");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "ApprovalRecords");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "ApprovalRecords");

            migrationBuilder.DropColumn(
                name: "IsUnderLegalHold",
                table: "ApplicationRoles");

            migrationBuilder.DropColumn(
                name: "LegalHoldAppliedAtUtc",
                table: "ApplicationRoles");

            migrationBuilder.DropColumn(
                name: "LegalHoldReason",
                table: "ApplicationRoles");
        }
    }
}
