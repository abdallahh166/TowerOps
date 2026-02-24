# Phase 0 - Project Charter (Historical Baseline)

This document is a historical baseline captured during mobilization. It records targets and assumptions used to start delivery. For current runtime behavior, use `docs/Api-Doc.md`, `docs/Application-Doc.md`, and `docs/Domain-Doc.md`.

## Problem Statement
Field maintenance previously depended on spreadsheet-heavy execution with weak traceability, inconsistent approvals, and delayed SLA visibility.

## Year-1 Objectives (Baseline)
- Digitize CM/BM execution
- Track end-to-end work order lifecycle
- Enforce evidence and approval flow
- Improve KPI visibility (SLA, MTTR, FTF, reopen)

## Baseline KPIs (Planning Targets)
- SLA compliance >= 95%
- MTTR <= 8 hours
- First-time-fix >= 90%
- Reopen rate <= 5%
- Evidence completeness >= 98%

## Initial Scope Baseline
- Work order lifecycle management
- Visit execution tracking
- Evidence upload and validation
- Escalation and approvals
- KPI dashboard baseline

## Initially Deferred in Planning Baseline
The following were initially marked deferred but are now partially/fully implemented in code:
- Offline sync
- Extended analytics/reporting

## Governance Baseline
Dual governance model with CM and BM responsibility lines (see `docs/phase-0/02-raci-matrix.md` and `docs/phase-0/06-approval-role-matrix.md`).

## Success Definition (Baseline)
1. Spreadsheet-only operation is replaced by controlled system workflows.
2. Work orders and visits are auditable end to end.
3. SLA and operational KPIs are measurable.
4. Approval responsibilities are explicit.
