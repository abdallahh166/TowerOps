using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderSlaClockByType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledVisitDateUtc",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaStartAtUtc",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkOrderType",
                table: "WorkOrders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "CM");

            migrationBuilder.Sql("""
                UPDATE WorkOrders
                SET SlaStartAtUtc = CreatedAt
                WHERE SlaStartAtUtc IS NULL;
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SlaStartAtUtc",
                table: "WorkOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_SlaStartAtUtc",
                table: "WorkOrders",
                column: "SlaStartAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_SlaStartAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledVisitDateUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "SlaStartAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "WorkOrderType",
                table: "WorkOrders");
        }
    }
}
