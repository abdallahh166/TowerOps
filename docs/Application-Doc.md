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
- `AddPhotoCommand`
- `AddReadingCommand`

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

/*
TelecomPM.Application/
├── Commands/
│   ├── Visits/
│   │   ├── CreateVisit/
│   │   │   ├── CreateVisitCommand.cs
│   │   │   ├── CreateVisitCommandHandler.cs
│   │   │   └── CreateVisitCommandValidator.cs
│   │   ├── StartVisit/
│   │   ├── CompleteVisit/
│   │   ├── SubmitVisit/
│   │   ├── ApproveVisit/
│   │   ├── RejectVisit/
│   │   ├── AddPhoto/
│   │   ├── AddReading/
│   │   └── LogMaterial/
│   │
│   ├── Sites/
│   │   ├── CreateSite/
│   │   ├── UpdateSite/
│   │   └── AssignSiteToEngineer/
│   │
│   ├── Materials/
│   │   ├── AddMaterial/
│   │   ├── UpdateStock/
│   │   └── ApproveMaterialUsage/
│   │
│   └── Users/
│       ├── CreateUser/
│       └── UpdateUser/
│
├── Queries/
│   ├── Visits/
│   │   ├── GetVisitById/
│   │   ├── GetEngineerVisits/
│   │   ├── GetPendingReviews/
│   │   └── GetScheduledVisits/
│   │
│   ├── Sites/
│   │   ├── GetSiteById/
│   │   ├── GetOfficeSites/
│   │   └── GetSitesNeedingMaintenance/
│   │
│   ├── Materials/
│   │   └── GetLowStockMaterials/
│   │
│   └── Reports/
│       ├── GetVisitReport/
│       └── GetMaterialConsumptionReport/
│
├── DTOs/
│   ├── Visits/
│   ├── Sites/
│   ├── Materials/
│   └── Common/
│
├── Mappings/
│   └── MappingProfile.cs
│
├── EventHandlers/
│   ├── VisitEventHandlers/
│   └── MaterialEventHandlers/
│
├── Services/
│   ├── Interfaces/
│   └── Implementations/
│
├── Behaviors/
│   ├── ValidationBehavior.cs
│   ├── LoggingBehavior.cs
│   └── PerformanceBehavior.cs
│
├── Exceptions/
│   └── ApplicationExceptions.cs
│
└── DependencyInjection.cs
*/