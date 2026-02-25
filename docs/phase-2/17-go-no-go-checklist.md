# Phase 2 Go/No-Go Checklist

Release: `v1.0.0-rc2`  
Date: 2026-02-25  
Change Window: Pending staging/prod window  
Owner: Abdullah Mahmoud

## A) Preconditions (Must be PASS before deploy)

- [ ] `docs/phase-2/12-production-remediation-plan.md` gates are all `PASS`
- [ ] CI green on release commit/tag
- [ ] `docs/phase-2/15-staging-smoke-and-rollback-verification.md` validated by QA
- [ ] Forward migration script available: `artifacts/phase-2/forward-idempotent.sql`
- [ ] Rollback script available: `artifacts/phase-2/rollback-from-latest.sql`
- [ ] DB backup snapshot taken and verified restorable
- [ ] Incident channel and on-call roster confirmed

## B) Deployment Execution

- [ ] Deploy API artifact/package to target environment
- [ ] Apply forward migration script
- [ ] Verify application startup health
- [ ] Verify configuration/secrets load (JWT, notifications, settings)

## C) Post-Deploy Smoke (Production/Staging)

- [x] Auth flow successful (`login`, `change password` as applicable)
- [x] Visit flow successful (`start`, `submit`, review path)
- [x] WorkOrder flow successful (`create`, `assign`, lifecycle transition)
- [x] Import flow successful (one controlled workbook import)
- [x] Portal flow successful (read-only scoped data access)
- [ ] No critical errors in logs for first 30 minutes

## D) Observability and Safety

- [ ] Correlation ID present in request/response and logs (`X-Correlation-ID`)
- [ ] Import/sync/notification metrics visible
- [ ] Alert rules active (per `docs/phase-2/13-observability-runbook.md`)
- [ ] Error contract responses follow unified schema

## E) Rollback Readiness Check

- [ ] Rollback trigger criteria reviewed with team
- [ ] DB rollback steps rehearsed and documented
- [ ] Rollback decision owner identified

Rollback trigger examples:
- Sustained 5xx spike > agreed threshold
- Data integrity issue detected
- Critical business workflow blocked

## F) Decision

- [ ] GO
- [x] NO-GO

Decision timestamp: 2026-02-25 (local validation)  
Decision notes: Local smoke suite is PASS. Final GO is blocked until staging deploy/migration is executed and runtime logs/health are verified.

## G) Sign-off

| Role | Name | Decision | Time |
|---|---|---|---|
| Backend Lead |  |  |  |
| QA Lead |  |  |  |
| DBA |  |  |  |
| Product Owner |  |  |  |
| Incident Commander |  |  |  |

## H) Local Smoke Evidence (2026-02-25)

- Auth smoke filter: PASS (11 tests)
- Visits smoke filter: PASS (32 tests; 16 app + 16 domain)
- WorkOrders smoke filter: PASS (20 tests; 4 app + 16 domain)
- Import smoke filter: PASS (18 tests)
- Portal smoke filter: PASS (11 tests; 7 app + 4 infra)

Executed commands match:
- `docs/phase-2/15-staging-smoke-and-rollback-verification.md`
