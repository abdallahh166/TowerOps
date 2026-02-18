# Phase 2 — Implementation Plan (v1)

## Timeline
- Duration: 4 sprints (2 weeks each)
- Start: after Phase-1 baseline freeze

## Sprint Sequencing

### Sprint 1 — Core Workflow Foundation
- WorkOrder create/assign APIs
- UTC timestamp policy in persistence
- Basic RBAC policy wiring
- Response SLA clock start

### Sprint 2 — Visit + Evidence
- Visit start/submit flows
- Readings/checklist/photo capture
- Evidence completeness validation
- Audit trail for visit actions

### Sprint 3 — Approval + Escalation
- CM/BM approval policy engine
- Reject→Rework loop
- Escalation API + hard mandatory-field validation
- Escalation routing rules

### Sprint 4 — SLA + KPI + Hardening
- Resolution/breach engine completion
- KPI endpoints and dashboard backend filters
- Security hardening (CORS/secrets/policy gaps)
- UAT fixes and release readiness

## Exit Criteria for Phase 2
- All Sprint goals delivered and validated.
- Zero blocker defects for UAT go-live.
- Full audit logs for lifecycle transitions.
- KPI endpoint outputs match business formulas.
