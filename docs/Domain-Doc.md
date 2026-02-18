# TelecomPM Domain Layer

## Overview
This is the core business logic layer implementing Clean Architecture and Domain-Driven Design principles.

## Key Components

### Aggregates
- **Site**: Telecom site with equipment and configuration
- **Visit**: Preventive/corrective maintenance visit
- **User**: Engineers and managers
- **Office**: Regional offices
- **Material**: Inventory items

### Value Objects
- `SiteCode`: Office code + sequence number
- `Coordinates`: GPS location with distance calculation
- `VoltageReading`: Validated voltage measurements
- `Money`: Currency-aware monetary values

### Domain Events
- `VisitSubmittedEvent`: Visit submitted for review
- `VisitApprovedEvent`: Visit approved by manager
- `LowStockAlertEvent`: Material stock below minimum

## Design Principles
- ✅ No infrastructure dependencies
- ✅ Rich domain model (not anemic)
- ✅ Encapsulation via private setters
- ✅ Validation in constructors/methods
- ✅ Domain events for side effects

## Usage Example
```csharp
// Create visit
var visit = Visit.Create(
    visitNumber: "V2025001",
    siteId: siteId,
    siteCode: "TNT001",
    siteName: "Tanta Central",
    engineerId: engineerId,
    engineerName: "Ahmed Hassan",
    scheduledDate: DateTime.Today,
    type: VisitType.PreventiveMaintenance
);

// Start visit
var location = Coordinates.Create(30.7865, 30.9925);
visit.StartVisit(location);

// Complete and submit
visit.CompleteVisit();
visit.Submit();

// Domain events raised automatically
// → VisitSubmittedEvent dispatched
```

## Testing
Run tests: `dotnet test TelecomPM.Domain.Tests`

## Dependencies
- None (pure domain logic)