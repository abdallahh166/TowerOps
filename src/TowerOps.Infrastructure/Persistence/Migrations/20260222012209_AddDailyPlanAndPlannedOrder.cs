using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyPlanAndPlannedOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlannedOrder",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DailyPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OfficeManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_DailyPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineerDayPlans",
                columns: table => new
                {
                    EngineerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalEstimatedDistanceKm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                    TotalEstimatedTravelMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineerDayPlans", x => new { x.DailyPlanId, x.EngineerId });
                    table.ForeignKey(
                        name: "FK_EngineerDayPlans_DailyPlans_DailyPlanId",
                        column: x => x.DailyPlanId,
                        principalTable: "DailyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlannedVisitStops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    SiteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteLatitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: false),
                    SiteLongitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: false),
                    VisitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DistanceFromPreviousKm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                    EstimatedTravelMinutes = table.Column<int>(type: "int", nullable: false),
                    DailyPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EngineerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedVisitStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedVisitStops_EngineerDayPlans_DailyPlanId_EngineerId",
                        columns: x => new { x.DailyPlanId, x.EngineerId },
                        principalTable: "EngineerDayPlans",
                        principalColumns: new[] { "DailyPlanId", "EngineerId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_OfficeId_PlanDate",
                table: "DailyPlans",
                columns: new[] { "OfficeId", "PlanDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannedVisitStops_DailyPlanId_EngineerId",
                table: "PlannedVisitStops",
                columns: new[] { "DailyPlanId", "EngineerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedVisitStops");

            migrationBuilder.DropTable(
                name: "EngineerDayPlans");

            migrationBuilder.DropTable(
                name: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "PlannedOrder",
                table: "Visits");
        }
    }
}
