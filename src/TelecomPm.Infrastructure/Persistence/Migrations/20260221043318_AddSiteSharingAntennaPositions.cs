using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomPm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSharingAntennaPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HostSiteCode",
                table: "SiteSharings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SharingRadioAntennaCount",
                table: "SiteSharings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SharingTxAntennaCount",
                table: "SiteSharings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TxEnclosureType",
                table: "SiteSharings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SiteSharingAntennaPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteSharingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Azimuth = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    HbaMeters = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSharingAntennaPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteSharingAntennaPositions_SiteSharings_SiteSharingId",
                        column: x => x.SiteSharingId,
                        principalTable: "SiteSharings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteSharingAntennaPositions_SiteSharingId",
                table: "SiteSharingAntennaPositions",
                column: "SiteSharingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSharingAntennaPositions");

            migrationBuilder.DropColumn(
                name: "HostSiteCode",
                table: "SiteSharings");

            migrationBuilder.DropColumn(
                name: "SharingRadioAntennaCount",
                table: "SiteSharings");

            migrationBuilder.DropColumn(
                name: "SharingTxAntennaCount",
                table: "SiteSharings");

            migrationBuilder.DropColumn(
                name: "TxEnclosureType",
                table: "SiteSharings");
        }
    }
}
