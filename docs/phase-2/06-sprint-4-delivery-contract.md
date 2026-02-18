# Sprint 4 — Delivery Contract (Execution)

## Scope
1. Runtime SLA monitoring through KPI backend query.
2. Unified operational KPI endpoint with filters (office/date/SLA class).
3. Security hardening baseline in API runtime (restricted CORS + visit review policies).
4. UAT-ready closure updates for Sprint 3 and Sprint 4 handoff.

## Delivered API Endpoints
- `GET /api/kpi/operations`

## Acceptance Criteria
- KPI endpoint returns SLA compliance, breach/at-risk counts, MTTR, FTF, reopen, and evidence completeness.
- KPI endpoint supports filters: `fromDateUtc`, `toDateUtc`, `officeCode`, `slaClass`.
- CORS policy is no longer open to all origins and is loaded from configuration.
- Visit approval/reject/request-correction actions enforce role-based policy (`CanReviewVisits`).

## Test Matrix
1. KPI endpoint happy path returns dashboard payload.
2. Escalation routing rejects mismatched escalation level.
3. Visit correction loop allows resubmission (`NeedsCorrection -> Submitted`).
4. CORS policy binds to configured allowed origins.

## Release Handoff
- Phase 2 exit verification and UAT execution with no blocker defects.


## Status Update
- ✅ `GET /api/kpi/operations` implemented with filter parameters (`fromDateUtc`, `toDateUtc`, `officeCode`, `slaClass`).
- ✅ `CanReviewVisits` and `CanViewKpis` policies are wired in authorization configuration.
- ✅ CORS remains configuration-driven (`Cors:AllowedOrigins`).

## Verification Notes
- Add integration tests for auth policy allow/deny flows per role.
- Validate UTC timestamp persistence with work order domain tests and persistence integration tests.
