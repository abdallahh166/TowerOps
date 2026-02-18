# TelecomPM — Phase 0 Mobilization Plan

## Objective
Start the project with a controlled foundation before implementation by locking:
1. Scope and governance
2. Core business definitions and data contracts
3. SLA/approval operating rules
4. Security and production guardrails

This phase is designed for the Mobiegypt × Orange Egypt subcontractor context and prepares the project for formal System Analysis (Phase 1).

---

## Duration
- **2 weeks** (10 working days)

---

## Phase 0 Deliverables (mandatory)

## D1) Project Charter
Must include:
- Problem statement
- Business goals and success criteria
- Scope in/out
- Constraints and assumptions
- Stakeholder map

## D2) Governance Pack
- RACI matrix
- RAID log (Risks, Assumptions, Issues, Dependencies)
- Decision log template
- Communication plan (daily/weekly cadence)

## D3) Data Contract v1
- Canonical keys across templates/systems:
  - `OfficeCode`, `SiteCode`, `VisitId`, `CapturedAt`
- Data Dictionary v1 (`FieldName`, `Definition`, `Type`, `AllowedValues`, `Owner`, `Source`)
- Master enum catalogs (status/severity/category values)

## D4) SLA & Approval Rules
- SLA matrix by priority/class (response & resolution targets)
- Breach policy definition
- Approval workflow:
  - Internal approval (Mobiegypt)
  - Customer acceptance (Orange side)

## D5) Security & Production Baseline
- Secrets management approach (no secrets in source)
- CORS policy constraints per environment
- Authorization policy matrix by role
- Error model baseline (structured error codes)

## D6) Phase 1 Readiness Pack
- AS-IS process draft
- TO-BE process draft
- Prioritized backlog for System Analysis
- Sign-off checklist

---

## Workplan (Day-by-day)

## Week 1
### Day 1 — Kickoff & Alignment
- Kickoff meeting with sponsor, ops, QA, engineering, and field representatives.
- Confirm project success metrics and delivery timeline.
- Open RAID log.

### Day 2 — Scope & Stakeholders
- Draft scope boundaries (in-scope / out-of-scope).
- Build stakeholder map and assign decision owners.

### Day 3 — Data Discovery
- Consolidate all workbook fields and current API/domain fields.
- Identify duplicates/conflicts in terms and labels.

### Day 4 — Definition Workshop
- Freeze top business definitions:
  - WO, Visit, SLA, Evidence Pack, Approval, FTF, MTTR.
- Start Data Dictionary v1.

### Day 5 — Governance Review
- Review RACI, communication cadence, and escalation model.
- Publish Week 1 status report.

## Week 2
### Day 6 — SLA Workshop
- Define SLA classes and target times.
- Define breach and exception handling rules.

### Day 7 — Approval & Role Matrix
- Finalize authorization responsibilities per role:
  - Dispatcher, Engineer, Supervisor, QA, Manager, Customer rep.
- Finalize approval chain and closure requirements.

### Day 8 — Security Baseline
- Approve secret management plan.
- Approve environment CORS profile.
- Approve authz policy baseline.

### Day 9 — Readiness Consolidation
- Package all outputs into Phase 1 readiness bundle.
- Prepare unresolved items list with owners and due dates.

### Day 10 — Steering Sign-off
- Steering committee review.
- Final sign-off / go-no-go for Phase 1.

---

## Roles and Responsibilities (Phase 0)
- **Project Sponsor:** Approves scope, priorities, and budget envelope.
- **Product Owner / Business Lead:** Owns business definitions and acceptance criteria.
- **Operations Lead:** Owns process practicality and field readiness.
- **QA Lead:** Defines evidence quality requirements and validation criteria.
- **Solution Architect:** Owns data contracts and system boundaries.
- **Security Lead:** Owns hardening baseline and policy controls.
- **Engineering Lead:** Owns feasibility and delivery sequencing.

---

## Entry Criteria
- Core business templates available.
- Key stakeholders nominated.
- Delivery team assigned.

## Exit Criteria (Go to Phase 1)
All below must be completed:
- [ ] Project Charter approved
- [ ] RACI + RAID + communication cadence approved
- [ ] Data Dictionary v1 approved
- [ ] Canonical keys approved
- [ ] SLA matrix approved
- [ ] Approval workflow approved
- [ ] Security baseline approved
- [ ] Phase 1 backlog approved

---

## Risks in Phase 0 (and mitigation)
1. **Conflicting definitions across teams**
   - Mitigation: single glossary owner + weekly decision forum.
2. **Unclear authority on approvals**
   - Mitigation: explicit role matrix and sign-off authority list.
3. **Data inconsistency across templates**
   - Mitigation: canonical key enforcement + mandatory dictionaries.
4. **Security hardening delayed**
   - Mitigation: make security baseline an exit criterion, not optional work.

---

## Artifacts Folder Structure
Recommended structure under `docs/phase-0/`:
- `01-project-charter.md`
- `02-raci-matrix.md`
- `03-raid-log.md`
- `04-data-dictionary-v1.xlsx`
- `05-sla-matrix-v1.xlsx`
- `06-approval-role-matrix.md`
- `07-security-baseline.md`
- `08-phase-1-readiness-checklist.md`

---

## Immediate Next Action
Start with **Day 1 Kickoff** and assign owners for D1–D6 deliverables before any new feature development.
