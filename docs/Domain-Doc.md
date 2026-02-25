# TowerOps Domain Layer Documentation

## Purpose
`TowerOps.Domain` contains the core business model and invariants. It is framework-agnostic and does not depend on infrastructure implementation details.

Branding note:
- Product brand: `TowerOps` (Seven Pictures)
- Internal domain assembly remains `TowerOps.Domain`

## Scope
- Aggregates and entities
- Value objects
- Domain events
- Domain exceptions
- Repository/service contracts
- Specifications and business enums

## Core Aggregates and Entities

### Operational entities
- `Site` (+ owned/related site component entities)
- `Visit` (+ photos, readings, checklist, issues, approvals, material usage)
- `WorkOrder`
- `Escalation`
- `Material`
- `Office`
- `User`

### Extended domain entities
- `ChecklistTemplate` and `ChecklistTemplateItem`
- `AuditLog`
- `ApprovalRecord`
- `BatteryDischargeTest`
- `DailyPlan`, `EngineerDayPlan`, `PlannedVisitStop`
- `Asset` and `AssetServiceRecord`
- `SyncQueue` and `SyncConflict`
- `SystemSetting`
- `ApplicationRole`
- `Client`
- `PasswordResetToken`
- `UnusedAsset`

## Value Objects
- Identifiers: `SiteCode`, `VisitNumber`, `MaterialCode`
- Location: `Coordinates`, `GeoLocation`, `Address`
- Contact: `Email`, `PhoneNumber`
- Other: `Money`, `MaterialQuantity`, `TimeRange`, `EvidencePolicy`, `Signature`

## Key Business Rules
- Work-order lifecycle transitions are guarded (`Created -> Assigned -> InProgress -> ...`).
- Visit lifecycle enforces status transitions and evidence policy before submit.
- Portal-facing ownership checks rely on `ClientCode` scope.
- Site ownership affects responsibility scope (`Host` => `Full`, otherwise `EquipmentOnly`).
- Equipment-only sites cannot create tower-infrastructure work orders.
- Signature capture enforces single-capture constraints and payload validation.
- Domain exceptions carry message keys for localization.

## Visit Type Canonical Model
- Canonical values: `BM`, `CM`
- Backward-compatible aliases exist in enum (`PreventiveMaintenance`, `CorrectiveMaintenance`) and are normalized by `VisitTypeExtensions`.

## Domain Events
Events are raised by aggregates and dispatched after successful persistence.
Examples:
- Visit lifecycle events (`VisitStartedEvent`, `VisitSubmittedEvent`, `VisitApprovedEvent`)
- Work order lifecycle events (`WorkOrderSubmittedForCustomerAcceptanceEvent`, `SlaBreachedEvent`)
- Site, material, checklist template, and asset events

## Data and Time Assumptions
- Business timestamps are UTC.
- Domain methods enforce invariant checks with explicit exceptions.
- Aggregate roots own mutation logic for child collections.

## Dependencies
- No dependency on ASP.NET Core, EF Core, or infrastructure services.
- Only pure .NET and domain contracts.

## Testing
Domain tests validate:
- state transitions
- invariants and guard conditions
- value object validation
- domain event emission

Run tests:
```bash
dotnet test tests/TowerOps.Domain.Tests/TowerOps.Domain.Tests.csproj
```

## Maintenance Notes
- Keep invariants inside aggregate methods.
- Avoid bypassing domain methods from application/infrastructure code.
- Add tests for every new transition or invariant.
