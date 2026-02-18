# Phase 0 — Approval & Role Matrix v1

## CM Workflow Approval Chain
1. Hardware Engineer executes visit.
2. CM Supervisor reviews evidence and checklist.
3. Office Manager performs approval.
4. Area Manager approves only in:
   - P1 cases
   - SLA breach cases
   - escalated disputes

## BM Workflow Approval Chain
1. BM Field Engineer executes PM/BM visit.
2. BM Monitoring Engineer reviews checklist/evidence.
3. BM Manager performs final approval.
4. Head of Maintenance participates only in escalations.

## Role Permissions (High-level)
| Role | Create WO | Execute Visit | Submit Evidence | Review | Approve | Escalate |
|---|---|---|---|---|---|---|
| Dispatcher / Coordinator | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Hardware Engineer (CM) | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| BM Field Engineer | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| CM Supervisor | ❌ | ❌ | ❌ | ✅ | ✅ (stage) | ✅ |
| BM Monitoring Engineer | ❌ | ❌ | ❌ | ✅ | ✅ (stage) | ✅ |
| Office Manager | ❌ | ❌ | ❌ | ✅ | ✅ (CM final except escalations) | ✅ |
| BM Manager | ❌ | ❌ | ❌ | ✅ | ✅ (BM final except escalations) | ✅ |
| Area Manager | ❌ | ❌ | ❌ | ✅ | ✅ (conditional) | ✅ |
| Head of Maintenance | ❌ | ❌ | ❌ | ✅ | ✅ (escalations) | ✅ |

## Design Constraints
- Parallel CM and BM approval hierarchies are mandatory.
- No hard-coded approval chains in code.
- All approval transitions must be policy/config driven.
- Full audit trail is mandatory for every transition.
