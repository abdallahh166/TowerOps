using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomPm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EngineerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOnDeviceUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConflictReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncConflicts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncQueueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConflictType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncConflicts_SyncQueues_SyncQueueId",
                        column: x => x.SyncQueueId,
                        principalTable: "SyncQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncConflicts_SyncQueueId",
                table: "SyncConflicts",
                column: "SyncQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncQueues_CreatedOnDeviceUtc",
                table: "SyncQueues",
                column: "CreatedOnDeviceUtc");

            migrationBuilder.CreateIndex(
                name: "IX_SyncQueues_DeviceId",
                table: "SyncQueues",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncQueues_DeviceId_EngineerId_OperationType_CreatedOnDeviceUtc",
                table: "SyncQueues",
                columns: new[] { "DeviceId", "EngineerId", "OperationType", "CreatedOnDeviceUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncQueues_EngineerId",
                table: "SyncQueues",
                column: "EngineerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncConflicts");

            migrationBuilder.DropTable(
                name: "SyncQueues");
        }
    }
}
