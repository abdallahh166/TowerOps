using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineerReportedCompletionTimeToVisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EngineerReportedCompletionTimeUtc",
                table: "Visits",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineerReportedCompletionTimeUtc",
                table: "Visits");
        }
    }
}
