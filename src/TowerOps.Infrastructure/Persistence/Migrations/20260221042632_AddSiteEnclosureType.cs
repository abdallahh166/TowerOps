using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteEnclosureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnclosureType",
                table: "Sites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnclosureTypeRaw",
                table: "Sites",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnclosureType",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "EnclosureTypeRaw",
                table: "Sites");
        }
    }
}
