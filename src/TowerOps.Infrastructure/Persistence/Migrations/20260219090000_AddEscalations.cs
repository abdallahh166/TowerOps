using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TowerOps.Infrastructure.Persistence;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260219090000_AddEscalations")]
    public partial class AddEscalations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escalations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SiteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SlaClass = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FinancialImpactEgp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SlaImpactPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EvidencePackage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PreviousActions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RecommendedDecision = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_Escalations", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_Escalations_IncidentId", table: "Escalations", column: "IncidentId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Escalations_Level", table: "Escalations", column: "Level");
            migrationBuilder.CreateIndex(name: "IX_Escalations_Status", table: "Escalations", column: "Status");
            migrationBuilder.CreateIndex(name: "IX_Escalations_SubmittedAtUtc", table: "Escalations", column: "SubmittedAtUtc");
            migrationBuilder.CreateIndex(name: "IX_Escalations_WorkOrderId", table: "Escalations", column: "WorkOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Escalations");
        }
    }
}
