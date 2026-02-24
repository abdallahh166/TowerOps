# Documentation Gap Report

## Summary
This report captures documentation areas that still need follow-up to keep docs fully synchronized with implementation details.

## Current Status
- Core runtime docs have been aligned:
  - `docs/Api-Doc.md`
  - `docs/Application-Doc.md`
  - `docs/Domain-Doc.md`
  - phase overview docs (`docs/phase-*/00-phase-overview.md`)
- Controller coverage drift check passes via `tools/check_doc_drift.py`.

## Remaining Gaps

### 1) Command/Query inventory granularity
- Gap: `docs/Application-Doc.md` is module-level; it does not list every individual command/query class.
- Impact: Medium (discoverability for new engineers).
- Recommendation: Generate command/query inventory from source into a doc artifact on CI.

### 2) DTO contract examples
- Gap: API doc lists routes but does not include full request/response JSON examples per endpoint.
- Impact: Medium (integration onboarding speed).
- Recommendation: Add example payloads for high-traffic endpoints (visits, workorders, portal, imports).

### 3) Historical sprint artifacts
- Gap: Several sprint/assessment docs are historical snapshots and may conflict with latest behavior.
- Impact: Medium (reader confusion).
- Recommendation: Keep them marked as historical and avoid treating them as runtime source of truth.

### 4) Data model ERD-level visualization
- Gap: No maintained ERD tied to latest EF migrations.
- Impact: Low to Medium (schema comprehension).
- Recommendation: Add generated ERD per release or migration milestone.

### 5) Operational runbooks
- Gap: Limited incident runbooks for production troubleshooting (auth failures, migration rollback strategy, queue conflict replay).
- Impact: Medium to High (operations readiness).
- Recommendation: Add runbooks under `docs/ops/`.

## Assumptions
- Source code and tests remain the final authority for behavior.
- Historical docs are retained for traceability, not for active implementation contracts.

## Follow-up Checklist
- [ ] Add auto-generated command/query catalog
- [ ] Add endpoint payload examples for critical routes
- [ ] Add ERD or schema map
- [ ] Add production runbooks
