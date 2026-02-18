# Phase 1 — Execution Baseline Confirmation (Locked)

## 1) Named Role Assignment

| Role | Assigned |
|---|---|
| Project Sponsor | Boda |
| Product Owner | Eng Saeed |
| Area Manager | Ahmed Saqa |
| Office Manager | El-Mohamed |
| BM Manager (Area-wide) | Moh Samir |
| Head of Maintenance | Eng Abdallah |
| Security Lead | Hamdy (Security) |
| Technical Lead (.NET) | Eng Abdallah |
| QA Lead | Eng Abdallah |

**Execution note:** Eng Abdallah currently holds 3 roles (Head of Maintenance, Technical Lead, QA Lead). Segregation-of-duties control must be monitored during implementation.

## 2) SLA & Severity Model (Frozen v1)

| Severity | Definition highlights | Response Target |
|---|---|---:|
| P1 (Critical) | Full site outage, safety risk, SLA breach forecast >20%, financial impact ≥ 100,000 EGP, regulatory violation | ≤ 1h |
| P2 (High) | Partial degradation, SLA impact 10–20%, financial impact 25k–100k, repeated rejection ≥ 2 cycles | ≤ 4h |
| P3 (Medium) | KPI deviation <10%, documentation gaps, evidence mismatch | ≤ 24h |
| P4 (Low) | Cosmetic/non-blocking, standard backlog | Backlog policy |

Status:
- Severity model approved.
- Escalation timeframes defined.

## 3) Escalation Governance (Rule-Based)

### CM → Area Manager escalation triggers
- Any P1
- P2 unresolved >24h
- Monthly SLA <90%
- FTF <80%
- Financial impact ≥ 50k EGP
- Repeated rejection ≥ 2 cycles

### Area Manager → BM escalation triggers
- Monthly SLA <85%
- Financial impact ≥ 250k EGP
- High-severity compliance issue
- Conflict unresolved
- Same issue appears in ≥3 sites within 30 days

### BM → Project Sponsor escalation triggers (Exceptional only)
- Strategic contract risk
- Budget overrun >15%
- Legal exposure
- Media/public impact

## 4) Escalation Time Policy (Frozen)

| Escalation Level | Max Delay |
|---|---:|
| P1 | ≤ 1h |
| P2 | 4h |
| P3 | 24h |
| BM escalation | ≤ 48h after Area review |

## 5) Escalation Data Requirement (Hard Rule)
No escalation request is accepted without:
- Incident ID
- Site Code
- Severity classification
- Financial estimate
- SLA impact %
- Evidence package
- Previous actions
- Recommended decision

System rule: submission must be blocked if any mandatory escalation field is missing.

## 6) Time Standard (Architecture Policy)
- All timestamps stored in UTC.
- Display converted to user-local timezone.
- `CapturedAt` mandatory.
- SLA clock based on UTC.

## 7) Phase 1 Kickoff
- Official start date: **25/03/2026**.

## 8) Gate Status

| Gate | Status |
|---|---|
| Named Roles | ✔ |
| SLA Matrix | ✔ |
| Escalation Rules | ✔ |
| UTC Policy | ✔ |
| Kickoff Date | ✔ |
