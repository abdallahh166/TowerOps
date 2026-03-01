# TowerOps Application Layer Documentation

## Purpose
`TowerOps.Application` orchestrates use cases using CQRS + MediatR. It contains no infrastructure implementation details and coordinates domain behavior through repositories, services, and unit of work abstractions.

Branding note:
- Product brand: `TowerOps` (Seven Pictures)
- Internal assembly/module name remains `TowerOps.Application`

## Scope
- Commands: state-changing workflows
- Queries: read workflows and DTO projections
- Validation and pipeline behaviors
- Mapping between transport contracts and application contracts
- Domain event handlers for cross-cutting actions

## Architecture

### Core folders
- `Common/`: results, interfaces, behaviors, base contracts
- `Commands/`: write use cases
- `Queries/`: read use cases
- `DTOs/`: response models for API/read models
- `EventHandlers/`: reactions to domain events
- `Mappings/`: AutoMapper profiles and contract mappers
- `Services/`: application-level services and policies

### Pipeline behaviors (execution order)
Configured in `DependencyInjection.cs`:
1. `UnhandledExceptionBehavior`
2. `LoggingBehavior`
3. `ValidationBehavior`
4. `PerformanceBehavior`
5. `TransactionBehavior`

## Functional Coverage

### Command modules
- `Auth`: login, forgot/reset/change password
- `Users`: create/update/delete, role change, activation/deactivation
- `Roles`: role CRUD + permission updates
- `Settings`: dynamic system settings update/test operations
- `Offices`: create/update/delete and contact update
- `Sites`: create/update/status/assign/unassign/ownership update/import flows
- `Visits`: lifecycle, evidence, checklist, issues, signatures, imports
- `WorkOrders`: lifecycle + customer acceptance + signatures
- `Escalations`: create/review/approve/reject/close
- `Materials`: CRUD + stock add/reserve/consume
- `DailyPlans`: create/assign/remove/suggest/publish
- `Assets`: register/service/fault/replace
- `Privacy`: authenticated self-service operational data export requests/download
- `Sync`: ingest offline batches and process conflicts
- `Reports`: scorecard/checklist/bdt/data-collection exports
- `AuditLogs` and `ApprovalRecords`: explicit audit/approval persistence commands

### Critical commands (drift-gated)
The following command contracts are checked by `tools/check_doc_drift.py` and must remain documented:
- `LogAuditEntryCommand`
- `CreateApprovalRecordCommand`
- `SubmitForCustomerAcceptanceCommand`
- `AcceptByCustomerCommand`
- `RejectByCustomerCommand`
- `ImportSiteDataCommand`
- `ImportSiteAssetsCommand`
- `ImportPowerDataCommand`
- `ImportSiteRadioDataCommand`
- `ImportSiteTxDataCommand`
- `ImportSiteSharingDataCommand`
- `ImportRFStatusCommand`
- `ImportBatteryDischargeTestCommand`
- `ImportDeltaSitesCommand`
- `ImportChecklistTemplateCommand`
- `ImportPanoramaEvidenceCommand`
- `ImportAlarmCaptureCommand`
- `ImportUnusedAssetsCommand`
- `GenerateContractorScorecardCommand`
- `ExportChecklistCommand`
- `ExportBDTCommand`
- `ExportDataCollectionCommand`
- `ExportScorecardCommand`
- `RequestMyOperationalDataExportCommand`

### Query modules
- `Portal`: client-scoped dashboard/sites/workorders/visits/SLA/evidence
- `Kpi`: operations dashboard metrics
- `Analytics`: performance, trends, issue analytics
- `Reports`: visit/report retrieval and export payloads
- `Signatures`: visit/work-order signature retrieval
- `Users`, `Sites`, `Visits`, `WorkOrders`, `Escalations`, `Materials`, `Offices`, `Roles`, `Settings`, `DailyPlans`, `Assets`, `Sync`, `Privacy`

## Business Logic Boundaries
- Handlers enforce use-case policy and orchestration.
- Domain entities enforce invariant and state-transition rules.
- Controllers must stay thin and call handlers via MediatR only.
- Read-heavy portal paths are delegated to `IPortalReadRepository` for DB-level filtering/projection.

## Dependencies
Main packages:
- MediatR
- FluentValidation
- AutoMapper
- Microsoft.AspNetCore.Identity (password hashing abstraction)

Infrastructure abstractions consumed:
- Repositories (`I*Repository`)
- `IUnitOfWork`
- `ISystemSettingsService`
- `ICurrentUserService`
- external interfaces (`IEmailService`, `INotificationService`, etc.)

## Edge Cases and Assumptions
- Input validation happens before handler logic.
- Authorization is enforced in API layer; handlers still apply ownership/scope checks where needed.
- Portal data must always be scoped by `ClientCode`.
- SLA values are read from settings at work-order creation and stored as immutable deadlines.
- All timestamps are treated as UTC.

## Configuration Touchpoints
The application layer behavior depends on settings such as:
- SLA values (`SLA:*`)
- Evidence policy (`Evidence:*`)
- Portal behavior (`Portal:*`)
- Route planning (`Route:*`)
- Sync behavior (`Sync:*`)

## Testing Strategy
- Unit tests per handler/service (happy path + guard/failure paths)
- Policy tests for authorization mappings
- Integration tests for import/export and reconciliation scenarios

Run tests:
```bash
dotnet test tests/TowerOps.Application.Tests/TowerOps.Application.Tests.csproj
```

## Maintenance Notes
- Keep command/query public contracts backward compatible unless versioning is introduced.
- Update this document when adding/removing command or query modules.
- Keep module-level details in code and tests as source of truth; this document is a curated map.
