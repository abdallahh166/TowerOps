using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitPhotoUploadSecurityWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileStatus",
                table: "VisitPhotos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Approved");

            migrationBuilder.AddColumn<string>(
                name: "QuarantineReason",
                table: "VisitPhotos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScanCompletedAtUtc",
                table: "VisitPhotos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitPhotos_FileStatus",
                table: "VisitPhotos",
                column: "FileStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitPhotos_FileStatus",
                table: "VisitPhotos");

            migrationBuilder.DropColumn(
                name: "FileStatus",
                table: "VisitPhotos");

            migrationBuilder.DropColumn(
                name: "QuarantineReason",
                table: "VisitPhotos");

            migrationBuilder.DropColumn(
                name: "ScanCompletedAtUtc",
                table: "VisitPhotos");
        }
    }
}
