# Phase 1 — Backlog v1 (Execution Ready)

## Epic 1 — Work Order Lifecycle
- Story 1.1: Create WO with SLA class and UTC timestamps
- Story 1.2: Assign WO and start response clock
- Story 1.3: Close WO with final gate validation

## Epic 2 — Visit Execution
- Story 2.1: Start visit and bind to WO
- Story 2.2: Capture readings + checklist
- Story 2.3: Upload evidence package and validate completeness

## Epic 3 — Approval Engine
- Story 3.1: CM workflow stages (Engineer → Supervisor → Office Manager)
- Story 3.2: BM workflow stages (Field Engineer → Monitoring Engineer → BM Manager)
- Story 3.3: Reject/Rework loop with audit persistence

## Epic 4 — SLA Engine
- Story 4.1: Response/Resolution clock service (UTC)
- Story 4.2: Breach detection + policy state
- Story 4.3: SLA KPI outputs (compliance, MTTR)

## Epic 5 — Escalation Engine
- Story 5.1: Rule-based trigger evaluation
- Story 5.2: Escalation submission hard validation
- Story 5.3: Escalation routing and timeline controls

## Epic 6 — Security & Audit
- Story 6.1: RBAC policy matrix implementation
- Story 6.2: Audit trail for create/update/approve/reject/escalate/close
- Story 6.3: Environment CORS and secrets hardening

## Sprint 1 Candidate Scope
- Epic 1 (stories 1.1, 1.2)
- Epic 2 (story 2.1)
- Epic 4 (story 4.1)
- Epic 6 (story 6.1)


## Phase 2 Handoff
- Architecture blueprint: `docs/phase-2/01-architecture-blueprint.md`
- Implementation plan: `docs/phase-2/02-implementation-plan.md`
- Sprint 1 contract: `docs/phase-2/03-sprint-1-delivery-contract.md`
