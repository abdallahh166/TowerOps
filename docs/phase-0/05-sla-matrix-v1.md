# Phase 0 — SLA Matrix v1 (Frozen)

## SLA Classes
| Class | Response Time | Severity Definition Highlights | Escalation Trigger Highlights |
|---|---:|---|---|
| P1 | ≤ 1h | Full site outage, safety risk, SLA breach forecast >20%, financial impact ≥100,000 EGP, regulatory violation | Immediate escalation path enabled |
| P2 | ≤ 4h | Partial degradation, SLA impact 10–20%, financial impact 25k–100k, repeated rejection ≥2 cycles | Escalate if unresolved >24h |
| P3 | ≤ 24h | KPI deviation <10%, documentation gaps, evidence mismatch | Standard supervisor escalation |
| P4 | Backlog policy | Cosmetic / non-blocking / standard backlog | Managed by planning backlog |

## Escalation Time Policy (Frozen)
| Escalation Level | Max Delay |
|---|---:|
| P1 | ≤ 1h |
| P2 | 4h |
| P3 | 24h |
| BM escalation | ≤ 48h after Area review |

## Rules
1. Every Work Order must have exactly one SLA class.
2. Response clock starts at WO creation time (UTC).
3. Resolution clock ends at final accepted closure (UTC).
4. Rejection returns WO to Rework and preserves SLA/audit history.
5. Escalation submission is blocked unless mandatory escalation fields are complete.
