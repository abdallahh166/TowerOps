# Mobiegypt × Orange Egypt — TelecomPM Operating Guide

## 1) What this project is (in simple terms)
TelecomPM is a field-maintenance management platform for telecom sites.
It helps Mobiegypt plan, execute, review, and report maintenance work done on Orange Egypt network sites.

In plain language:
- **Office teams** plan and assign maintenance work.
- **Field engineers** execute site visits (with readings, photos, checklist evidence).
- **Supervisors/reviewers** validate and approve/reject submissions.
- **Management** tracks KPIs, productivity, quality, and material usage.

---

## 2) Core business definitions (important)

## 2.1 Work Order (WO)
A formal maintenance request (usually from NOC/planning/customer operations).

**Recommended fields**:
- WO Number
- Customer (Orange)
- Site Code / Site ID
- Priority (P1/P2/P3)
- SLA response time
- SLA resolution time
- Required maintenance scope
- Creation time / due time / closure time

> Note: Current system is centered on `Visit`. For production subcontractor operations, adding an explicit `WorkOrder` entity is strongly recommended.

## 2.2 Visit
A field execution instance performed by engineer(s) at a site.
- Created with engineer + site + schedule.
- Moves through lifecycle states (scheduled, started, completed, submitted, reviewed).
- Contains evidence: photos, readings, checklist, issues, approvals.

## 2.3 SLA (Service Level Agreement)
The contractual timing and quality rules that Mobiegypt must meet.

**Common SLA measures**:
- **Response Time**: time from WO creation to engineer arrival/start.
- **Resolution Time**: time from WO creation/start to accepted closure.
- **Breach**: any case exceeding SLA threshold.

## 2.4 Evidence Pack
All proof artifacts required to close a task:
- Before/After photos
- Time and location stamps
- Technical readings
- Checklist completion
- Issue and fix notes

## 2.5 Approval
Formal quality/operations validation of completed work.

Recommended approval levels:
1. **Internal approval** (Mobiegypt supervisor)
2. **Customer acceptance** (Orange representative or delegated team)

## 2.6 Repeat Issue
A fault recurring at same site/equipment within a defined window (e.g., 7/14/30 days).
Used to measure quality and root-cause effectiveness.

## 2.7 First Time Fix (FTF)
WO solved correctly in the first field intervention without reopen/repeat issue in the defined window.

## 2.8 MTTR (Mean Time To Repair)
Average time taken to resolve incidents/work orders.

Formula:
`MTTR = Sum(resolution times) / Number of resolved WOs`

---

## 3) Target operating model for Mobiegypt

## 3.1 End-to-end process
1. **WO Intake**
   - Receive work order from planning/NOC/customer channel.
2. **Dispatching & Assignment**
   - Assign office, engineer, schedule, and SLA class.
3. **Field Execution**
   - Engineer starts visit on-site with location/time proof.
   - Collects readings, photos, checklist, and issue logs.
4. **Submission**
   - Engineer submits completed visit.
5. **Quality Review**
   - Supervisor approves/rejects/requests correction.
6. **Customer Acceptance**
   - Optional/required final acceptance by Orange side.
7. **Closure & Reporting**
   - KPI update, SLA compliance record, and archival of evidence.

## 3.2 Role definitions (RACI-ready)
- **Dispatcher / Coordinator**: creates/assigns work, tracks SLA clocks.
- **Field Engineer**: executes visit and submits evidence pack.
- **Supervisor / QA**: validates quality and compliance before closure.
- **Warehouse/Logistics**: supports material reservation/consumption.
- **Operations Manager**: tracks KPI and escalates SLA risks.
- **Customer Interface (Orange side)**: acceptance and service quality sign-off.

---

## 4) KPI dictionary (with definitions)

## 4.1 SLA Compliance %
Percentage of closed WOs delivered within SLA.

Formula:
`SLA Compliance % = (WO closed within SLA / Total closed WO) × 100`

## 4.2 SLA Breach %
Percentage of WOs that violated SLA.

Formula:
`SLA Breach % = (WO breached / Total closed WO) × 100`

## 4.3 First Time Fix Rate %
Percentage of WOs solved without repeat/reopen inside observation window.

Formula:
`FTF % = (WO fixed first time / Total WO closed) × 100`

## 4.4 Average Response Time
Average time between WO creation and visit start.

## 4.5 MTTR
Average time to close repair/maintenance WO.

## 4.6 Reopen/Repeat Rate %
Percentage of WOs reopened or repeated.

## 4.7 Evidence Completeness %
Percentage of closed visits with full required evidence (photos/readings/checklist/location).

## 4.8 Material Cost per WO
Average consumed material value per WO.

---

## 5) Gap analysis against current implementation

## 5.1 Strong capabilities already available
- Visit lifecycle management (create/start/complete/submit/review actions).
- Photo and reading capture.
- Maintenance/site/material/user analytics endpoints.
- Low-stock visibility for materials.

## 5.2 Key gaps for subcontractor production model
1. **Work Order aggregate exists (baseline), needs maturity hardening**
   - `WorkOrder` aggregate and APIs are implemented; next step is deepening SLA/audit/acceptance governance.
2. **No explicit customer acceptance stage**
   - Add acceptance status/event after supervisor approval.
3. **SLA clocks not first-class domain concept**
   - Add SLA policy model + breach computation pipeline.
4. **Role/policy hardening needed for production governance**
   - Enforce strict role-based endpoint access and approvals.
5. **Contract-ready KPI pack still partial**
   - Add fixed KPI definitions and dashboard-level governance.

---

## 6) Suggested domain refactoring (clear and actionable)

## 6.1 New aggregates/value objects
- `WorkOrder` aggregate
- `SlaPolicy` value object/entity
- `CustomerAcceptance` entity
- `EvidencePolicy` (minimum requirements by visit type/site type)

## 6.2 Lifecycle proposal
`WorkOrder` states:
- `Created` → `Assigned` → `InProgress` → `PendingInternalReview` → `PendingCustomerAcceptance` → `Closed`
- plus exceptional states: `Rejected`, `Cancelled`, `Reopened`

## 6.3 Mapping to existing Visit model
- Keep Visit as execution unit.
- Add relation: `WorkOrder 1..N Visit` (or `1..1` initially if operations require).
- Keep current photos/readings/checklist/approval data under Visit.

---

## 7) 30-60-90 implementation plan

## 7.1 First 30 days (stabilize business alignment)
- Freeze KPI definitions with Mobiegypt operations and Orange stakeholders.
- Define SLA matrix and evidence policy by work type.
- Add BRD section: contractual closure criteria.

## 7.2 Day 31-60 (domain and API extension)
- Implement WorkOrder aggregate and APIs.
- Add customer acceptance workflow and status.
- Add SLA breach computation and events.

## 7.3 Day 61-90 (governance and scaling)
- Publish operations dashboard (SLA, FTF, MTTR, reopen).
- Add role-based policy matrix and audit trails.
- Add monthly contractor scorecard export package.

---

## 8) Practical glossary (quick reference)
- **Subcontractor**: service provider contracted by operator (Mobiegypt for Orange scope).
- **NOC**: Network Operations Center (monitoring/escalation origin).
- **Preventive Maintenance (PM)**: scheduled checks to prevent failures.
- **Corrective Maintenance (CM)**: reactive fixes after faults.
- **Escalation**: routing high-risk/SLA-threatening cases to higher authority.
- **RCA**: Root Cause Analysis for repeated/critical issues.
- **Dispatching**: assigning work by region/skill/load.

---

## 9) What to share with stakeholders tomorrow
Use this short message:

> "TelecomPM already supports visit execution and review. To align with Mobiegypt contractual operations with Orange Egypt, we will introduce explicit Work Orders, SLA policy enforcement, customer acceptance, and a fixed KPI governance model (SLA, FTF, MTTR, repeat rate)."



## 10) Execution kickoff
- Start immediately with `docs/Phase-0-Mobilization-Plan.md` as the official mobilization track before feature delivery.
