using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomPm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStockValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentStockUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinimumStockValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumStockUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReorderQuantityValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReorderQuantityUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UnitCostAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCostCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastRestockDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Offices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressRegion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<double>(type: "float(11)", precision: 11, scale: 8, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TotalSites = table.Column<int>(type: "int", nullable: false),
                    ActiveEngineers = table.Column<int>(type: "int", nullable: false),
                    ActiveTechnicians = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Offices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxAssignedSites = table.Column<int>(type: "int", nullable: true),
                    Specializations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PerformanceRating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    AssignedSiteIds = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EngineerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EngineerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TechnicianNames = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Visit_ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Visit_ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CheckInLatitude = table.Column<double>(type: "float(10)", precision: 10, scale: 8, nullable: true),
                    CheckInLongitude = table.Column<double>(type: "float(11)", precision: 11, scale: 8, nullable: true),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReadingsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsPhotosComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsChecklistComplete = table.Column<bool>(type: "bit", nullable: false),
                    CompletionPercentage = table.Column<int>(type: "int", nullable: false),
                    EngineerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SupervisorNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_Visits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConsumed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialReservations_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StockBefore = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StockBeforeUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StockAfter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StockAfterUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                    table.PrimaryKey("PK_MaterialTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialTransactions_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiteOfficeCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SiteSequenceNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OMCName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubRegion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 7, nullable: false),
                    Longitude = table.Column<double>(type: "float(10)", precision: 10, scale: 7, nullable: false),
                    AddressStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressRegion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SiteType = table.Column<int>(type: "int", nullable: false),
                    Complexity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AnnouncementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BSCName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BSCCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subcontractor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaintenanceArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstimatedVisitDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    LastVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredPhotosCount = table.Column<int>(type: "int", nullable: false),
                    AssignedEngineerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Users_AssignedEngineerId",
                        column: x => x.AssignedEngineerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VisitApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_VisitApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitApprovals_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitChecklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RelatedPhotoIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_VisitChecklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitChecklists_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PhotoIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RequiresFollowUp = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_VisitIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitIssues_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitMaterialUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCostCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BeforePhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AfterPhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_VisitMaterialUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitMaterialUsages_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThumbnailPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<double>(type: "float(11)", precision: 11, scale: 8, nullable: true),
                    VisitId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_VisitPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitPhotos_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitPhotos_Visits_VisitId1",
                        column: x => x.VisitId1,
                        principalTable: "Visits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisitReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadingType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinAcceptable = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxAcceptable = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    IsWithinRange = table.Column<bool>(type: "bit", nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phase = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_VisitReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitReadings_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteCoolingSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ACUnitsCount = table.Column<int>(type: "int", nullable: false),
                    HasVentilation = table.Column<bool>(type: "bit", nullable: false),
                    HasDCACUnit = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACUnits = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteCoolingSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteCoolingSystems_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteFireSafeties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirePanelType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeatSensorsCount = table.Column<int>(type: "int", nullable: false),
                    SmokeSensorsCount = table.Column<int>(type: "int", nullable: false),
                    FlameSensorsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extinguishers = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteFireSafeties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteFireSafeties_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitePowerSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Configuration = table.Column<int>(type: "int", nullable: false),
                    RectifierBrand = table.Column<int>(type: "int", nullable: false),
                    RectifierModulesCount = table.Column<int>(type: "int", nullable: false),
                    RectifierControllerType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BatteryType = table.Column<int>(type: "int", nullable: false),
                    BatteryStrings = table.Column<int>(type: "int", nullable: false),
                    BatteriesPerString = table.Column<int>(type: "int", nullable: false),
                    BatteryAmpereHour = table.Column<int>(type: "int", nullable: false),
                    BatteryVoltage = table.Column<int>(type: "int", nullable: false),
                    HasSolarPanel = table.Column<bool>(type: "bit", nullable: false),
                    SolarPanelWatt = table.Column<int>(type: "int", nullable: true),
                    SolarPanelsCount = table.Column<int>(type: "int", nullable: true),
                    HasGenerator = table.Column<bool>(type: "bit", nullable: false),
                    GeneratorType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GeneratorSerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GeneratorKVA = table.Column<int>(type: "int", nullable: true),
                    FuelTankSizeLiters = table.Column<int>(type: "int", nullable: true),
                    HasPowerMeter = table.Column<bool>(type: "bit", nullable: false),
                    PowerMeterRate = table.Column<int>(type: "int", nullable: true),
                    ElectricityPhaseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_SitePowerSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitePowerSystems_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteRadioEquipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Has2G = table.Column<bool>(type: "bit", nullable: false),
                    Has3G = table.Column<bool>(type: "bit", nullable: false),
                    Has4G = table.Column<bool>(type: "bit", nullable: false),
                    HasSRAN = table.Column<bool>(type: "bit", nullable: false),
                    BTSVendor = table.Column<int>(type: "int", nullable: true),
                    BTSType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BTSCount = table.Column<int>(type: "int", nullable: true),
                    TwoGModulesCount = table.Column<int>(type: "int", nullable: true),
                    NodeBVendor = table.Column<int>(type: "int", nullable: true),
                    NodeBType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThreeGRadioModules = table.Column<int>(type: "int", nullable: true),
                    ThreeGTransmissionModules = table.Column<int>(type: "int", nullable: true),
                    FourGModulesCount = table.Column<int>(type: "int", nullable: true),
                    SectorsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sectors = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteRadioEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteRadioEquipments_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteSharings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsShared = table.Column<bool>(type: "bit", nullable: false),
                    HostOperator = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GuestOperators = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PowerShared = table.Column<bool>(type: "bit", nullable: false),
                    TowerShared = table.Column<bool>(type: "bit", nullable: false),
                    HasSharingLock = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SiteSharings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteSharings_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteTowerInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberOfMasts = table.Column<int>(type: "int", nullable: false),
                    WiresPerMast = table.Column<int>(type: "int", nullable: false),
                    NeedsRepair = table.Column<bool>(type: "bit", nullable: false),
                    WiresNeedRepair = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SiteTowerInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteTowerInfos_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteTransmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LinksCount = table.Column<int>(type: "int", nullable: false),
                    HasGPS = table.Column<bool>(type: "bit", nullable: false),
                    HasADM = table.Column<bool>(type: "bit", nullable: false),
                    HasSDH = table.Column<bool>(type: "bit", nullable: false),
                    HasEBand = table.Column<bool>(type: "bit", nullable: false),
                    HasALURouter = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MWLinks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTransmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteTransmissions_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReservations_MaterialId_IsConsumed",
                table: "MaterialReservations",
                columns: new[] { "MaterialId", "IsConsumed" });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialReservations_VisitId",
                table: "MaterialReservations",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Code",
                table: "Materials",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_IsActive",
                table: "Materials",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_OfficeId",
                table: "Materials",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransactions_MaterialId",
                table: "MaterialTransactions",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransactions_TransactionDate",
                table: "MaterialTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransactions_Type",
                table: "MaterialTransactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTransactions_VisitId",
                table: "MaterialTransactions",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_Code",
                table: "Offices",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offices_IsActive",
                table: "Offices",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_Region",
                table: "Offices",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_SiteCoolingSystems_SiteId",
                table: "SiteCoolingSystems",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteFireSafeties_SiteId",
                table: "SiteFireSafeties",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SitePowerSystems_SiteId",
                table: "SitePowerSystems",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteRadioEquipments_SiteId",
                table: "SiteRadioEquipments",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_AssignedEngineerId",
                table: "Sites",
                column: "AssignedEngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Complexity",
                table: "Sites",
                column: "Complexity");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_OfficeId",
                table: "Sites",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_OfficeId_Status",
                table: "Sites",
                columns: new[] { "OfficeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Region",
                table: "Sites",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteCode",
                table: "Sites",
                column: "SiteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Status",
                table: "Sites",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSharings_SiteId",
                table: "SiteSharings",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteTowerInfos_SiteId",
                table: "SiteTowerInfos",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteTransmissions_SiteId",
                table: "SiteTransmissions",
                column: "SiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OfficeId",
                table: "Users",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_VisitApprovals_Action",
                table: "VisitApprovals",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_VisitApprovals_ReviewerId",
                table: "VisitApprovals",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitApprovals_VisitId",
                table: "VisitApprovals",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitChecklists_Category",
                table: "VisitChecklists",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_VisitChecklists_Status",
                table: "VisitChecklists",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VisitChecklists_VisitId",
                table: "VisitChecklists",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitIssues_Severity",
                table: "VisitIssues",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_VisitIssues_Status",
                table: "VisitIssues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VisitIssues_VisitId",
                table: "VisitIssues",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMaterialUsages_MaterialId",
                table: "VisitMaterialUsages",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMaterialUsages_Status",
                table: "VisitMaterialUsages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VisitMaterialUsages_VisitId",
                table: "VisitMaterialUsages",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPhotos_Category",
                table: "VisitPhotos",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPhotos_Type",
                table: "VisitPhotos",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPhotos_VisitId",
                table: "VisitPhotos",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPhotos_VisitId1",
                table: "VisitPhotos",
                column: "VisitId1");

            migrationBuilder.CreateIndex(
                name: "IX_VisitReadings_Category",
                table: "VisitReadings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_VisitReadings_ReadingType",
                table: "VisitReadings",
                column: "ReadingType");

            migrationBuilder.CreateIndex(
                name: "IX_VisitReadings_VisitId",
                table: "VisitReadings",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_EngineerId",
                table: "Visits",
                column: "EngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ScheduledDate",
                table: "Visits",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_SiteId",
                table: "Visits",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_Status",
                table: "Visits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_VisitNumber",
                table: "Visits",
                column: "VisitNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialReservations");

            migrationBuilder.DropTable(
                name: "MaterialTransactions");

            migrationBuilder.DropTable(
                name: "Offices");

            migrationBuilder.DropTable(
                name: "SiteCoolingSystems");

            migrationBuilder.DropTable(
                name: "SiteFireSafeties");

            migrationBuilder.DropTable(
                name: "SitePowerSystems");

            migrationBuilder.DropTable(
                name: "SiteRadioEquipments");

            migrationBuilder.DropTable(
                name: "SiteSharings");

            migrationBuilder.DropTable(
                name: "SiteTowerInfos");

            migrationBuilder.DropTable(
                name: "SiteTransmissions");

            migrationBuilder.DropTable(
                name: "VisitApprovals");

            migrationBuilder.DropTable(
                name: "VisitChecklists");

            migrationBuilder.DropTable(
                name: "VisitIssues");

            migrationBuilder.DropTable(
                name: "VisitMaterialUsages");

            migrationBuilder.DropTable(
                name: "VisitPhotos");

            migrationBuilder.DropTable(
                name: "VisitReadings");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
