using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TowerOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGpsCheckInToVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CheckInGeoLatitude",
                table: "Visits",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CheckInGeoLongitude",
                table: "Visits",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTimeUtc",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CheckOutLatitude",
                table: "Visits",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CheckOutLongitude",
                table: "Visits",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutTimeUtc",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistanceFromSiteMeters",
                table: "Visits",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWithinSiteRadius",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AllowedCheckInRadiusMeters",
                table: "Sites",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInGeoLatitude",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "CheckInGeoLongitude",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "CheckInTimeUtc",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "CheckOutLatitude",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "CheckOutLongitude",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "CheckOutTimeUtc",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "DistanceFromSiteMeters",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "IsWithinSiteRadius",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "AllowedCheckInRadiusMeters",
                table: "Sites");
        }
    }
}
