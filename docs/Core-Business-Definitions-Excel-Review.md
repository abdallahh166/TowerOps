# Core Business Definitions - Excel Review

This document is an input-data review snapshot. It should be read together with `docs/Excel-Domain-Gap-Report.md` and current import command implementations.

This document summarizes the three Excel workbooks added under `docs/` for core business definitions and operational templates.

## Reviewed files

1. `docs/GH- DE BDT.xlsx`
2. `docs/GH-DE Checklist.xlsx`
3. `docs/GH-DE Data Collection.xlsx`

---

## 1) GH- DE BDT.xlsx

**Purpose (inferred):** Battery discharge, power alarm logging, and high-level weekly summary tracking.

**Sheets reviewed:**
- `BDT sheet` — battery discharge test form with site/subcontractor metadata.
- `Power Alarm` — alarm-oriented engineering capture.
- `Config` — template/site configuration helper area.
- `Summary` — roll-up table (week, site, date, nodal degree, PLVD/LLVD, linked sites).

### Strengths
- Good operational focus for power-domain maintenance.
- Contains both detail-level capture (`BDT sheet`) and roll-up (`Summary`).
- Includes configuration/helper section to reduce manual re-entry.

### Gaps / Risks
- Missing explicit data dictionary row in each sheet (field meaning, allowed values).
- Weekly summary fields need strict controlled values (e.g., nodal degree taxonomy).
- Alarm capture and BDT measurements should share a stable Site key (`SiteCode` + `OfficeCode`) to avoid reconciliation errors.

### Recommendations
1. Add a fixed **Data Dictionary** sheet in workbook with: `FieldName`, `Definition`, `Type`, `AllowedValues`, `Owner`, `Source`.
2. Add validation lists for alarm type, severity, and test outcome.
3. Enforce unique row key format: `Week + SiteCode + TestDate + EngineerId`.

---

## 2) GH-DE Checklist.xlsx

**Purpose (inferred):** Site audit, physical inspection, panorama/photo capture, and reserves/pending observations.

**Sheets reviewed:**
- `site's reading`
- `Common checklist`
- `Panorama`
- `Tower Panorama`
- `Before & after`
- `Pending Res.`
- `unused assets`
- `alarms capture`
- `Audit matrix SQI`
- `Sheet1` / `Sheet2` (present but effectively unused placeholders)

### Strengths
- Broad end-to-end field audit coverage (readings + visual evidence + pending actions).
- Good separation between checklist evidence and remediation tracking (`Pending Res.`).
- Dedicated SQI audit matrix suggests quality scoring capability.

### Gaps / Risks
- `Sheet1`/`Sheet2` placeholders increase confusion and risk accidental data entry.
- Photo sheets should enforce mandatory metadata: `Timestamp`, `Geo`, `PhotoType`, `AssetRef`.
- Pending items require SLA columns (`TargetDate`, `Owner`, `Status`, `ClosureProof`).

### Recommendations
1. Archive or hide unused sheets (`Sheet1`, `Sheet2`) and keep named operational tabs only.
2. Standardize checklist status values: `OK`, `NOK`, `NA`.
3. Add remediation SLA tracking in `Pending Res.` and link each pending item to visit/checklist reference.
4. Add conditional formatting for overdue pending items.

---

## 3) GH-DE Data Collection.xlsx

**Purpose (inferred):** Central structured dataset for sharing, transmission, radio, power, and asset counts.

**Sheets reviewed:**
- `Site Sharing Data`
- `Site TX Data`
- `Site Radio Data` (largest operational table)
- `Power Data`
- `Site Assets Data Count`
- `RF Status`

### Strengths
- Strong domain decomposition by subsystem (sharing/TX/radio/power/assets/RF).
- Suitable foundation for integration to reporting and analytics.
- Large radio table indicates good granularity for technical operations.

### Gaps / Risks
- Cross-sheet key consistency may break if different naming is used (`Site Name` vs `Site Code`).
- `RF Status` should align to one canonical status catalog.
- Asset count sheet should separate **count snapshot date** from **record update date**.

### Recommendations
1. Define a **Canonical Key Set** across all sheets: `OfficeCode`, `SiteCode`, `VisitId`, `CapturedAt`.
2. Create a shared lookup sheet for enums: `RFStatus`, `TXType`, `PowerState`, `AlarmSeverity`.
3. Add versioning fields for technical baseline comparison: `BaselineVersion`, `ChangeReason`.

---

## Cross-workbook findings (important)

## A) Definition Governance
- Add one master glossary workbook/sheet and reference it from all templates.
- Every operational field should map to a single business definition.

## B) Data Quality Controls
- Add drop-downs and input constraints for all categorical fields.
- Add format validation for IDs, phone/email, dates, and numeric ranges.

## C) Integration Readiness (TelecomPM mapping)
Map workbook fields to system entities:
- Site metadata → `Site`
- Visit execution/checklist/photos/readings → `Visit`, `VisitChecklist`, `VisitPhoto`, `VisitReading`
- Pending remediation/issues → `VisitIssue`
- Power/radio/TX structured data → Site component entities + analytics queries

## D) Operational KPIs to derive from these templates
- Evidence completeness %
- Pending resolution overdue %
- Alarm recurrence rate
- Site power-risk index
- First-time-pass audit rate

---

## Priority action plan

### P0 (Immediate)
1. Remove/disable placeholder tabs and lock template structure.
2. Introduce canonical keys and mandatory columns across all sheets.
3. Add controlled vocabularies for statuses and alarm severities.

### P1 (Next sprint)
1. Add data dictionary + glossary tabs and ownership metadata.
2. Add SLA tracking columns for pending resolutions.
3. Build import mapping spec from Excel fields to TelecomPM DTOs/entities.

### P2 (Stabilization)
1. Add automated validation and ingestion pipeline.
2. Add monthly quality dashboard (completeness, consistency, overdue, recurrence).
3. Deprecate manual summary sheets when analytics API coverage is complete.

---

## Notes
- Workbook structures are multi-sheet and appear intended for field data capture plus audit/reporting handoff.
- Template/formula placeholder ranges are acceptable, but should be governance-controlled to avoid accidental edits.
