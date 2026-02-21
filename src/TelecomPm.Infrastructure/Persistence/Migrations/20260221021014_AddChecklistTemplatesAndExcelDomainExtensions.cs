using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomPm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistTemplatesAndExcelDomainExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChecklistTemplateId",
                table: "Visits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChecklistTemplateVersion",
                table: "Visits",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDateUtc",
                table: "VisitIssues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateItemId",
                table: "VisitChecklists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BandForRFOnGround",
                table: "Sites",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BandForRFOnTower",
                table: "Sites",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralNotes",
                table: "Sites",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RFOnGroundCount",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RFOnTowerCount",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RFSectorCarryCount",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RFStatusNotes",
                table: "Sites",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteShortCode",
                table: "Sites",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRFCount",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZTEMonitoringStatus",
                table: "Sites",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatteryBrand",
                table: "SitePowerSystems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatteryHealthStatus",
                table: "SitePowerSystems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CabinetVendor",
                table: "SitePowerSystems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCabinetized",
                table: "SitePowerSystems",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModemCount",
                table: "SitePowerSystems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouterCount",
                table: "SitePowerSystems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BatteryDischargeTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedVisitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EngineerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubcontractorOffice = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Network = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiteCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PowerSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NodalDegree = table.Column<int>(type: "int", nullable: true),
                    StartVoltage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StartAmperage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EndVoltage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EndAmperage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PlvdLlvdValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DischargeTimeMinutes = table.Column<int>(type: "int", nullable: true),
                    ReasonForStop = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReasonForRepeatedBDT = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CapRequestNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Week = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryDischargeTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFromUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveToUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangeNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    ApplicableSiteTypes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApplicableVisitTypes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplateItems_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ChecklistTemplateId",
                table: "Visits",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitIssues_TargetDateUtc",
                table: "VisitIssues",
                column: "TargetDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_VisitChecklists_TemplateItemId",
                table: "VisitChecklists",
                column: "TemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryDischargeTests_SiteId",
                table: "BatteryDischargeTests",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryDischargeTests_TestDateUtc",
                table: "BatteryDischargeTests",
                column: "TestDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplateItems_ChecklistTemplateId",
                table: "ChecklistTemplateItems",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplateItems_OrderIndex",
                table: "ChecklistTemplateItems",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_VisitType_EffectiveFromUtc",
                table: "ChecklistTemplates",
                columns: new[] { "VisitType", "EffectiveFromUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_VisitType_IsActive",
                table: "ChecklistTemplates",
                columns: new[] { "VisitType", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatteryDischargeTests");

            migrationBuilder.DropTable(
                name: "ChecklistTemplateItems");

            migrationBuilder.DropTable(
                name: "ChecklistTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Visits_ChecklistTemplateId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_VisitIssues_TargetDateUtc",
                table: "VisitIssues");

            migrationBuilder.DropIndex(
                name: "IX_VisitChecklists_TemplateItemId",
                table: "VisitChecklists");

            migrationBuilder.DropColumn(
                name: "ChecklistTemplateId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ChecklistTemplateVersion",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "TargetDateUtc",
                table: "VisitIssues");

            migrationBuilder.DropColumn(
                name: "TemplateItemId",
                table: "VisitChecklists");

            migrationBuilder.DropColumn(
                name: "BandForRFOnGround",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "BandForRFOnTower",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "GeneralNotes",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "RFOnGroundCount",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "RFOnTowerCount",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "RFSectorCarryCount",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "RFStatusNotes",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "SiteShortCode",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "TotalRFCount",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ZTEMonitoringStatus",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "BatteryBrand",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "BatteryHealthStatus",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "CabinetVendor",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "IsCabinetized",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "ModemCount",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "RouterCount",
                table: "SitePowerSystems");
        }
    }
}
