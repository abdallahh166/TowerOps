# Phase 2 Staging Smoke and Rollback Verification

Date: 2026-02-24  
Owner: QA / Backend / DBA

## 1) Scope

This evidence file closes the final release gates in `docs/phase-2/12-production-remediation-plan.md`:
- Staging smoke coverage for `Auth`, `Visits`, `WorkOrders`, `Import`, `Portal`
- Rollback and migration verification

Execution was performed on a staging-equivalent local environment using the same solution/test targets and real Excel integration assets from `docs/excell/*`.

## 2) Smoke Test Evidence

## Command set

```powershell
dotnet test TowerOps.sln --configuration Debug --no-build --filter "FullyQualifiedName~AuthControllerTests|FullyQualifiedName~LoginCommandHandlerTests|FullyQualifiedName~ForgotPasswordCommandHandlerTests|FullyQualifiedName~ResetPasswordCommandHandlerTests|FullyQualifiedName~ChangePasswordCommandHandlerTests|FullyQualifiedName~ChangePasswordCommandValidatorTests" --logger "console;verbosity=minimal"

dotnet test TowerOps.sln --configuration Debug --no-build --filter "FullyQualifiedName~StartVisitCommandHandlerTests|FullyQualifiedName~SubmitVisitCommandHandlerTests|FullyQualifiedName~CancelVisitCommandHandlerTests|FullyQualifiedName~RescheduleVisitCommandHandlerTests|FullyQualifiedName~VisitQueryEfficiencyTests|FullyQualifiedName~GetVisitEvidenceStatusQueryHandlerTests|FullyQualifiedName~VisitTests|FullyQualifiedName~VisitLifecycleTests|FullyQualifiedName~VisitReviewFlowTests|FullyQualifiedName~VisitEditCancelRulesTests" --logger "console;verbosity=minimal"

dotnet test TowerOps.sln --configuration Debug --no-build --filter "FullyQualifiedName~CreateWorkOrderCommandHandlerTests|FullyQualifiedName~CustomerDecisionPortalGuardTests|FullyQualifiedName~WorkOrderTests" --logger "console;verbosity=minimal"

dotnet test TowerOps.sln --configuration Debug --no-build --filter "FullyQualifiedName~ImportSiteDataCommandHandlerTests|FullyQualifiedName~ImportCommandsRealFilesIntegrationTests|FullyQualifiedName~Sprint12DryRunReconciliationTests" --logger "console;verbosity=minimal"

dotnet test TowerOps.sln --configuration Debug --no-build --filter "FullyQualifiedName~PortalQueriesTests|FullyQualifiedName~PortalReadRepositoryTests|FullyQualifiedName~CustomerDecisionPortalGuardTests" --logger "console;verbosity=minimal"
```

## Results summary

| Area | Result |
|---|---|
| Auth smoke | PASS (11 tests) |
| Visits smoke | PASS (32 tests) |
| WorkOrders smoke | PASS (20 tests) |
| Import smoke (real files + reconciliation) | PASS (18 tests) |
| Portal smoke | PASS (11 tests) |
| Total staged smoke subset | PASS (92 tests) |

Related reconciliation artifact:
- `docs/phase-2/10-sprint-12-dry-run-reconciliation.md`

## 3) Rollback and Migration Verification

## Migration inventory

Command:

```powershell
dotnet ef migrations list --project src/TowerOps.Infrastructure/TowerOps.Infrastructure.csproj --startup-project src/TowerOps.Api/TowerOps.Api.csproj
```

Result:
- Latest migration: `20260223225829_AddResidualGapFieldsAndUnusedAssets`
- Previous migration: `20260222025332_AddTowerOwnershipToSite`

## Script generation evidence

Commands:

```powershell
dotnet ef migrations script --idempotent --project src/TowerOps.Infrastructure/TowerOps.Infrastructure.csproj --startup-project src/TowerOps.Api/TowerOps.Api.csproj --output artifacts/phase-2/forward-idempotent.sql

dotnet ef migrations script 20260223225829_AddResidualGapFieldsAndUnusedAssets 20260222025332_AddTowerOwnershipToSite --project src/TowerOps.Infrastructure/TowerOps.Infrastructure.csproj --startup-project src/TowerOps.Api/TowerOps.Api.csproj --output artifacts/phase-2/rollback-from-latest.sql
```

Produced artifacts:
- `artifacts/phase-2/forward-idempotent.sql`
- `artifacts/phase-2/rollback-from-latest.sql`

## Rehearsal execution (apply + rollback)

Command flow:
1. Create temporary LocalDB database: `TowerOps_RollbackSmoke_ccdeddd0`
2. Apply all migrations up to latest.
3. Roll back from latest to previous migration.
4. Drop temporary database.

Observed result:
- Migration apply: PASS
- Migration revert (latest down): PASS
- DB cleanup: PASS

## 4) Observed Warnings (Non-blocking, tracked)

During EF update, warnings were emitted for:
- Enum default sentinel configuration on `Site.ResponsibilityScope`, `Site.TowerOwnershipType`, `WorkOrder.Scope`
- Decimal precision not explicitly set on `Escalation.FinancialImpactEgp` and `Escalation.SlaImpactPercentage`

These are not rollback blockers and did not fail migration execution, but should be addressed in a follow-up hardening PR.

## 5) Gate Decision

- `Staging smoke test (Auth, Visits, WorkOrders, Import, Portal) executed` -> PASS
- `Rollback and migration verification signed off` -> PASS
