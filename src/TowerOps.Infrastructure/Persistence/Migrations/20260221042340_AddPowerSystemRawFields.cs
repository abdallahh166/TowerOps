using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPowerSystemRawFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PowerSourceLabel",
                table: "SitePowerSystems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RectifierBrandRaw",
                table: "SitePowerSystems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PowerSourceLabel",
                table: "SitePowerSystems");

            migrationBuilder.DropColumn(
                name: "RectifierBrandRaw",
                table: "SitePowerSystems");
        }
    }
}
