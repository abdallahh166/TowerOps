# TelecomPM Release Notes - v1.0.0-rc1

Date: 2026-02-24  
Release Owner: Engineering

## 1) Summary

This release closes Phase 2 production remediation gates and marks the system as release-candidate ready.

## 2) Highlights

- Production remediation plan gates are closed with evidence:
  - `docs/phase-2/12-production-remediation-plan.md`
  - `docs/phase-2/15-staging-smoke-and-rollback-verification.md`
- Observability and operational baselines are documented:
  - `docs/phase-2/13-observability-runbook.md`
  - `docs/phase-2/14-performance-baseline.md`
- API error contract is unified (stable code/message/correlation model).
- Documentation drift checks were expanded and gated in CI.

## 3) Quality and Verification

- CI workflow: `.NET CI`
- Latest successful run evidence:  
  `https://github.com/boda166/telecomPm/actions/runs/22373242789`
- Local full test run evidence at release prep:
  - `dotnet test TelecomPM.sln --configuration Debug --no-build --logger "console;verbosity=minimal"`
  - Result: `404 passed, 0 failed`

## 4) Migration and Rollback

- Migration inventory validated via `dotnet ef migrations list`.
- Forward idempotent script generated:
  - `artifacts/phase-2/forward-idempotent.sql`
- Rollback script generated:
  - `artifacts/phase-2/rollback-from-latest.sql`
- Rehearsal executed (apply latest -> rollback latest -> drop temp DB) and passed:
  - `docs/phase-2/15-staging-smoke-and-rollback-verification.md`

## 5) Operational Notes

- Staging smoke coverage executed for Auth, Visits, WorkOrders, Import, Portal.
- Non-blocking technical warnings tracked for follow-up:
  - Enum default sentinel configuration warnings (EF)
  - Decimal precision configuration for escalation fields

## 6) Breaking Changes

- None identified in this release candidate.

## 7) Follow-up Items (Post-RC)

- Add explicit EF precision for escalation decimal columns.
- Review enum default sentinel strategy for DB defaults.
