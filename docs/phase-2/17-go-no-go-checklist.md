# Phase 2 Go/No-Go Checklist

Release: `v1.0.0-rc1`  
Date: __________  
Change Window: __________  
Owner: __________

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

- [ ] Auth flow successful (`login`, `change password` as applicable)
- [ ] Visit flow successful (`start`, `submit`, review path)
- [ ] WorkOrder flow successful (`create`, `assign`, lifecycle transition)
- [ ] Import flow successful (one controlled workbook import)
- [ ] Portal flow successful (read-only scoped data access)
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
- [ ] NO-GO

Decision timestamp: __________  
Decision notes: __________________________________________

## G) Sign-off

| Role | Name | Decision | Time |
|---|---|---|---|
| Backend Lead |  |  |  |
| QA Lead |  |  |  |
| DBA |  |  |  |
| Product Owner |  |  |  |
| Incident Commander |  |  |  |
