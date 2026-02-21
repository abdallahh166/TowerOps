# Sprint 12 - Import/Export Hardening + UAT Readiness

Date: 2026-02-21

## Scope
1. Freeze Sprint 10/11 import/export baseline in source control.
2. Add real-workbook integration tests using files in `docs/excell`.
3. Close remaining checklist/evidence import gaps:
   - `ImportChecklistTemplateCommand`
   - `ImportPanoramaEvidenceCommand`
   - `ImportAlarmCaptureCommand`
4. Produce dry-run reconciliation report with imported/skipped/error and entity-diff output.
5. Run release gate validation (`dotnet build` + `dotnet test`) and refresh docs.

## Delivered API Additions
- `POST /api/checklisttemplates/import` (**CanManageWorkOrders**)
- `POST /api/visits/{visitId}/import/panorama`
- `POST /api/visits/{visitId}/import/alarms`

## Delivered Application Additions
- New import commands:
  - `ImportChecklistTemplateCommand`
  - `ImportPanoramaEvidenceCommand`
  - `ImportAlarmCaptureCommand`
- Real-file integration coverage:
  - `ImportCommandsRealFilesIntegrationTests`
  - `ExportCommandsRealFilesIntegrationTests`
- Dry-run reconciliation generator test:
  - `Sprint12DryRunReconciliationTests`

## Acceptance Criteria
- Real Excel files can be parsed by all import/export handlers without runtime exceptions.
- Gap-report checklist/evidence import commands are implemented and API-exposed.
- Reconciliation report is generated under `docs/phase-2/`.
- Build and tests pass before merge.

## Evidence
- API docs: `docs/Api-Doc.md`
- Dry-run report: `docs/phase-2/10-sprint-12-dry-run-reconciliation.md`
