# Documentation Index and Source-of-Truth Map

## Purpose
This index classifies project documentation by maintenance level so the team can quickly identify what is authoritative versus historical context.

## Live (Authoritative)
These documents are intended to match current code behavior:
- `README.md`
- `docs/Api-Doc.md`
- `docs/Application-Doc.md`
- `docs/Domain-Doc.md`
- `docs/Operational-Workflow.md`
- `docs/Frontend-Reporting-API-Contract-and-Git-Plan.md`
- `docs/Backend-Auto-Discovery-Report.md`
- `docs/Business-Confirmation-Checklist.md`
- `docs/TowerOps-Brand-Profile.md`
- `docs/phase-0/00-phase-overview.md`
- `docs/phase-1/00-phase-overview.md`
- `docs/phase-2/00-phase-overview.md`
- `docs/phase-2/07-definition-of-done-checklist.md`
- `docs/phase-2/08-release-readiness-report.md`
- `docs/phase-2/10-sprint-12-dry-run-reconciliation.md`
- `docs/phase-2/11-residual-gap-checklist.md`
- `docs/phase-2/12-production-remediation-plan.md`
- `docs/phase-2/13-observability-runbook.md`
- `docs/phase-2/14-performance-baseline.md`
- `docs/phase-2/15-staging-smoke-and-rollback-verification.md`
- `docs/phase-2/16-release-notes-v1.0.0-rc1.md`
- `docs/phase-2/17-go-no-go-checklist.md`
- `docs/phase-2/18-business-confirmation-implementation-pr-slices.md`
- `docs/phase-2/19-bc05-staging-rollout-verification.md`

## Historical / Planning Snapshots
These remain useful for context but can diverge from runtime implementation:
- `docs/Phase-0-Mobilization-Plan.md`
- `docs/phase-0/*` (except `00-phase-overview.md`)
- `docs/phase-1/*` (except `00-phase-overview.md`)
- `docs/phase-2/01-architecture-blueprint.md` to `06-sprint-4-delivery-contract.md`
- `docs/phase-2/09-sprint-12-delivery-contract.md`
- `docs/Architecture-Consistency-Review.md`
- `docs/Architecture-Decision-Freeze.md`
- `docs/Comprehensive-Project-Review.md`
- `docs/Senior-Architecture-Assessment.md`

## Dataset and Audit Inputs
- `docs/excell/*` (field Excel templates)
- `docs/Excel-Domain-Gap-Report.md`
- `docs/Core-Business-Definitions-Excel-Review.md`
- `docs/Core-Business-Definitions-Workbook-Audit.md`

## Maintenance Rule
When code changes impact behavior:
1. Update live docs first (`Api-Doc`, `Application-Doc`, `Domain-Doc`, workflow).
2. Run `python tools/check_doc_drift.py`.
3. Update `docs/Documentation-Gap-Report.md` for any intentional or pending mismatches.
