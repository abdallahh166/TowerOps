using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTowerOwnershipToSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "WorkOrders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "ClientEquipment");

            migrationBuilder.AddColumn<string>(
                name: "HostContactName",
                table: "Sites",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostContactPhone",
                table: "Sites",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsibilityScope",
                table: "Sites",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Full");

            migrationBuilder.AddColumn<string>(
                name: "SharingAgreementRef",
                table: "Sites",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TowerOwnerName",
                table: "Sites",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TowerOwnershipType",
                table: "Sites",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Host");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "HostContactName",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "HostContactPhone",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ResponsibilityScope",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "SharingAgreementRef",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "TowerOwnerName",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "TowerOwnershipType",
                table: "Sites");
        }
    }
}
