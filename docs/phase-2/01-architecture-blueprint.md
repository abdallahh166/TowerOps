# Phase 2 — Architecture Blueprint (v1)

## Objective
Translate Phase-1 analysis outputs into implementation-ready technical architecture with enforceable policies for:
- WorkOrder lifecycle
- CM/BM parallel approval hierarchies
- SLA clock + breach engine
- Escalation policy engine
- Evidence validation and audit trail

## Bounded Components
1. **WorkOrder Service**
   - Owns WO state machine and identifiers.
2. **Visit Execution Service**
   - Owns readings, checklist, photos, evidence completeness.
3. **Approval Engine**
   - Config-driven policy model for CM and BM flows.
4. **SLA Engine**
   - UTC-based response/resolution timers and breach evaluation.
5. **Escalation Engine**
   - Rule evaluation and mandatory data validation before escalation.
6. **KPI Aggregation Layer**
   - SLA compliance, MTTR, FTF, reopen rate, evidence completeness.

## Data Model Baseline
- `WorkOrders`
- `Visits`
- `VisitReadings`
- `VisitChecklistItems`
- `VisitPhotos`
- `ApprovalRecords`
- `EscalationRecords`
- `SlaSnapshots`
- `AuditLogs`

## Critical Technical Rules
1. Store all timestamps in UTC.
2. Block escalation API submission on missing mandatory fields.
3. No hardcoded approval chain in handlers/controllers.
4. Persist immutable breach/audit history.
5. Enforce role + stage policy checks on all workflow actions.

## Security Controls (Design-time)
- JWT authentication + policy-based authorization.
- Secrets externalized from source control.
- Environment-specific CORS profiles.
- Audit record for every state transition.

## NFR Targets
- API p95 latency < 500ms for core WO operations.
- 99.9% service availability target (production).
- Full traceability for lifecycle actions (who/when/what).
