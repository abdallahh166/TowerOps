# Phase 0 — RAID Log (Initial)

## Risks
| ID | Risk | Impact | Mitigation | Owner |
|---|---|---|---|---|
| R1 | Continued Excel usage by field teams | High | Make system mandatory at closure gate; disable manual closure without system evidence | Product Owner |
| R2 | Approval ambiguity between CM/BM hierarchies | High | Configure separate approval chains and escalation rules by workflow type | Solution Architect |
| R3 | SLA model disagreements (P1..P4) | High | Finalize SLA matrix in governance workshop and freeze v1 | Area Manager + BM Manager |
| R4 | Inconsistent keys across data sources | High | Enforce canonical keys (`OfficeCode`,`SiteCode`,`VisitId`,`CapturedAt`) | Architect |
| R5 | Security hardening delayed | High | Gate Phase 1 start by RBAC and secrets baseline sign-off | Security Lead |
| R6 | Segregation-of-duties risk (same person in multiple critical roles) | Medium | Add compensating controls: independent approval witness + weekly governance review | Sponsor + Security Lead |

## Assumptions
| ID | Assumption | Validation Method | Owner |
|---|---|---|---|
| A1 | All work orders will be captured digitally | Pilot in one area/office | Product Owner |
| A2 | CM/BM governance owners are available weekly | Weekly steering attendance | Sponsor |
| A3 | KPI targets are agreed for Year 1 | Signed KPI sheet | Area Manager + BM Manager |

## Issues
| ID | Issue | Status | Action | Owner |
|---|---|---|---|---|
| I1 | Current process has no real-time SLA breach monitoring | Open | Introduce SLA engine in Phase 1 scope | Eng Lead |
| I2 | Weak component-level history traceability | Open | Add component-linked visit/issue model | Architect |

## Dependencies
| ID | Dependency | Needed By | Owner |
|---|---|---|---|
| D1 | Approved SLA classes and times | Phase 1 Analysis | Area Manager + BM Manager |
| D2 | RBAC role catalog | Phase 1 Architecture | Security Lead |
| D3 | Approval matrices (CM/BM) | Workflow design | Product Owner |
