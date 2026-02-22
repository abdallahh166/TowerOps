using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomPm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalSignatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientSignatureBase64",
                table: "WorkOrders",
                type: "nvarchar(max)",
                maxLength: 250000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClientSignedAtUtc",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClientSignedLatitude",
                table: "WorkOrders",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClientSignedLongitude",
                table: "WorkOrders",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSignerName",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSignerPhone",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSignerRole",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineerSignatureBase64",
                table: "WorkOrders",
                type: "nvarchar(max)",
                maxLength: 250000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EngineerSignedAtUtc",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineerSignedLatitude",
                table: "WorkOrders",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineerSignedLongitude",
                table: "WorkOrders",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineerSignerName",
                table: "WorkOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineerSignerPhone",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngineerSignerRole",
                table: "WorkOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteContactSignatureBase64",
                table: "Visits",
                type: "nvarchar(max)",
                maxLength: 250000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SiteContactSignedAtUtc",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SiteContactSignedLatitude",
                table: "Visits",
                type: "decimal(10,8)",
                precision: 10,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SiteContactSignedLongitude",
                table: "Visits",
                type: "decimal(11,8)",
                precision: 11,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteContactSignerName",
                table: "Visits",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteContactSignerPhone",
                table: "Visits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteContactSignerRole",
                table: "Visits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSignatureBase64",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientSignedAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientSignedLatitude",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientSignedLongitude",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientSignerName",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientSignerPhone",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClientSignerRole",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignatureBase64",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignedAtUtc",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignedLatitude",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignedLongitude",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignerName",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignerPhone",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EngineerSignerRole",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "SiteContactSignatureBase64",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SiteContactSignedAtUtc",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SiteContactSignedLatitude",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SiteContactSignedLongitude",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SiteContactSignerName",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SiteContactSignerPhone",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SiteContactSignerRole",
                table: "Visits");
        }
    }
}
