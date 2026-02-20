# TelecomPM.Application Layer

## Overview
This is the Application Layer implementing CQRS pattern with MediatR, complete with validation, logging, and transaction management.

## Structure
```
TelecomPM.Application/
├── Common/               # Base classes, interfaces, behaviors
├── Commands/             # Write operations (CQRS)
├── Queries/              # Read operations (CQRS)
├── DTOs/                 # Data Transfer Objects
├── EventHandlers/        # Domain event handlers
├── Mappings/             # AutoMapper profiles
└── DependencyInjection.cs
```

## Key Features

### CQRS Pattern
- **Commands**: Modify state (Create, Update, Delete)
- **Queries**: Read-only operations

### Pipeline Behaviors
1. **LoggingBehavior**: Logs all requests/responses
2. **ValidationBehavior**: FluentValidation integration
3. **PerformanceBehavior**: Detects slow queries (>500ms)
4. **TransactionBehavior**: Auto-transaction management

### Result Pattern
```csharp
// Success
return Result.Success(dto);

// Failure
return Result.Failure("Error message");
```

### Event Handlers
- Automatic notifications on domain events
- Email and push notifications
- Async processing

## Usage

### In Program.cs
```csharp
builder.Services.AddApplication();
```

### In Controller
```csharp
[HttpPost]
public async Task<IActionResult> CreateVisit(CreateVisitCommand command)
{
    var result = await _mediator.Send(command);
    
    if (result.IsFailure)
        return BadRequest(new { error = result.Error });
        
    return Ok(result.Value);
}
```

## Commands Available

### Visit Commands
- `CreateVisitCommand`
- `StartVisitCommand`
- `CompleteVisitCommand`
- `SubmitVisitCommand`
- `ApproveVisitCommand`
- `RejectVisitCommand`
- `RequestCorrectionCommand`
- `CancelVisitCommand`
- `RescheduleVisitCommand`
- `AddPhotoCommand`
- `RemovePhotoCommand`
- `AddReadingCommand`
- `UpdateReadingCommand`

### WorkOrder Commands
- `CreateWorkOrderCommand`
- `AssignWorkOrderCommand`
- `StartWorkOrderCommand`
- `CompleteWorkOrderCommand`
- `CloseWorkOrderCommand`
- `CancelWorkOrderCommand`
- `SubmitForCustomerAcceptanceCommand`
- `AcceptByCustomerCommand`
- `RejectByCustomerCommand`

### Escalation Commands
- `CreateEscalationCommand`
- `ReviewEscalationCommand`
- `ApproveEscalationCommand`
- `RejectEscalationCommand`
- `CloseEscalationCommand`

### Material Commands
- `CreateMaterialCommand`
- `UpdateMaterialCommand`
- `DeleteMaterialCommand`
- `AddStockCommand`
- `ReserveStockCommand`
- `ConsumeStockCommand`

### Audit & Approval Commands (Sprint 5/6)
- `LogAuditEntryCommand`
- `CreateApprovalRecordCommand`

### Queries Available

### Visit Queries
- `GetVisitByIdQuery`
- `GetEngineerVisitsQuery` (paginated)
- `GetPendingReviewsQuery`
- `GetScheduledVisitsQuery`

### Site Queries
- `GetSiteByIdQuery`
- `GetOfficeSitesQuery` (paginated)
- `GetSitesNeedingMaintenanceQuery`

### Material Queries
- `GetLowStockMaterialsQuery`

### Report Queries
- `GetVisitReportQuery`

## Dependencies
```xml
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
```

## Testing
```csharp
[Fact]
public async Task CreateVisit_WithValidData_ReturnsSuccess()
{
    // Arrange
    var command = new CreateVisitCommand { ... };
    var handler = new CreateVisitCommandHandler(...);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
}
```

## Notes

- All commands are validated using FluentValidation
- All operations are logged
- Slow queries are automatically detected
- Transactions are managed automatically
- Domain events trigger notifications

## Current Command/Query Coverage (high-level)

Implemented command groups include:
- Visits (lifecycle, evidence, checklist, issues)
- WorkOrders (create, assign)
- Escalations (create with routing validation)
- Users (CRUD + role + activation/deactivation + specializations)
- Offices (CRUD + statistics + contact)
- Materials (create/update/reserve/consume/approve/restock)

Implemented query groups include:
- Visits (detail, engineer visits, pending reviews, scheduled, evidence status)
- Sites (by office/engineer/region/complexity/unassigned/maintenance)
- WorkOrders and Escalations (by id)
- Reports/Analytics/KPI (operations dashboard, trends, performance, usage summaries)
- Users/Materials/Offices with operational reporting views

## Documentation maintenance rule
- Treat this file as release-level summary, and rely on code structure as source of truth for exact command/query inventory.
