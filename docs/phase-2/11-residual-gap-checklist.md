# Residual Gap Checklist (Post Sprint 12)

Date: 2026-02-23

Source of truth reviewed:
- `docs/Excel-Domain-Gap-Report.md` (Section 4 + Section 5)
- Current implementation in `src/` + integration tests in `tests/TowerOps.Application.Tests/Integration/`

## Summary
- **High-priority gap recommendations from the original report are mostly implemented.**
- Remaining gaps from the original sprint are now implemented in Sprint 13 scope.
- Import/export coverage from real files is in place and validated in Sprint 12 tests.

## 1) Domain/Schema Gap Status

| Gap | Status | Notes |
|---|---|---|
| `Site.OperationalZone` | Done | Implemented and used in delta/site imports. |
| `Site.GeneralNotes` | Done | Implemented; can cover contextual free text. |
| `SitePowerSystem.IsCabinetized` + cabinet vendor | Done | Implemented as `IsCabinetized`, `CabinetVendor`. |
| `SitePowerSystem.BatteryHealthStatus` | Done | Implemented and populated by imports. |
| `MWLink` detailed RF/TX fields | Done | Management IP, frequencies, power, modulation, configuration, polarization, ODU serials, azimuth/HBA are implemented. |
| `SectorInfo` band + feeder support | Done | Implemented as `BandLabel`, `FeederSize`, `FeederLengthM`, etc. |
| `SiteSharing` per-antenna positions | Done | Implemented via `SharedAntennaPosition`. |
| `VisitIssue.TargetDateUtc` | Done | Implemented. |
| `BatteryDischargeTest` aggregate | Done | Implemented with import + export paths. |
| `Site.LegacyShortCode` explicit field | Done | Added as nullable persisted field and populated in import flows. |
| `Site.ExternalContextNotes` explicit field | Done | Added as nullable persisted field; monitoring/general import updates it. |
| `SitePowerSystem.CabinetType` explicit field | Done | Added and wired in power import parsing. |
| `SitePowerSystem.ChargingCurrentLimit` | Done | Added and populated from BDT import when available. |
| `VisitPhoto.CapturedAtUtc` explicit mapping | Done | Added explicit field with DTO/export fallback handling. |
| `UnusedAsset` aggregate | Done | New aggregate + repository + export integration + import command from checklist workbook. |

## 2) Import Readiness Gap Status

| Gap Report Item | Current Status | Notes |
|---|---|---|
| `ImportSiteAssetsCommand` | Done | Implemented + real-file integration tests. |
| `ImportPowerDataCommand` | Done | Implemented + real-file integration tests. |
| `ImportSiteRadioDataCommand` | Done | Implemented + real-file integration tests. |
| `ImportSiteTxDataCommand` | Done | Implemented + real-file integration tests. |
| `ImportSiteSharingDataCommand` | Done | Implemented + real-file integration tests. |
| `ImportRFStatusCommand` | Done | Implemented + real-file integration tests. |
| `ImportBatteryDischargeTestCommand` | Done | Implemented + real-file integration tests. |
| `ImportChecklistTemplateCommand` | Done | Implemented + real-file integration tests. |
| `ImportPanoramaEvidenceCommand` | Done | Implemented + real-file integration tests. |
| `ImportAlarmCaptureCommand` | Done | Implemented + real-file integration tests. |
| `ImportUnusedAssetsCommand` | Done | Implemented from `unused assets` sheet with real-file integration coverage. |
| `ImportDeltaBatteryStatusCommand` (named in report) | Superseded | Covered by `ImportDeltaSitesCommand` pipeline. |
| `ImportPowerInventoryCommand` (named in report) | Superseded | Covered by `ImportPowerDataCommand` pipeline. |

## 3) Sprint 13 Delivery Status

1. Explicit schema fields
   - Delivered: `Site.LegacyShortCode`, `Site.ExternalContextNotes`, `SitePowerSystem.CabinetType`, `SitePowerSystem.ChargingCurrentLimit`, `VisitPhoto.CapturedAtUtc`.

2. `UnusedAsset` aggregate
   - Delivered: aggregate + repository + EF config + DI wiring.
   - Delivered: import command from `unused assets` sheet.
   - Delivered: export integration in checklist export flow.

3. Migration + compatibility
   - Delivered: additive migration `AddResidualGapFieldsAndUnusedAssets`.
   - No destructive rename/drop operations introduced.

4. Tests
   - Delivered: domain tests for `UnusedAsset` and `VisitPhoto` captured timestamp behavior.
   - Delivered: integration tests for unused-assets import and export paths.

5. Documentation
   - Delivered: API docs updated for new unused-assets visit import endpoint.
   - Delivered: residual checklist status updated to current implementation.

## 4) Release Gate for Sprint 13

- `dotnet build TowerOps.sln`
- `dotnet test TowerOps.sln --logger "console;verbosity=minimal"`
- `python tools/check_doc_drift.py`
- Dry-run reconciliation refresh under `docs/phase-2/`
