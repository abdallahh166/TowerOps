using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomPm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNodalDegree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MWLinkCount",
                table: "SiteTransmissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NodalDegreeRaw",
                table: "SiteTransmissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MWLinkCount",
                table: "SiteTransmissions");

            migrationBuilder.DropColumn(
                name: "NodalDegreeRaw",
                table: "SiteTransmissions");
        }
    }
}
