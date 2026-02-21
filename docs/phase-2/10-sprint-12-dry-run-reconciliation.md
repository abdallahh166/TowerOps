# Sprint 12 Dry-Run Reconciliation Report

Generated At UTC: 2026-02-21 06:54:35

## Coverage Summary
| Command | Source File | Success | Imported | Skipped |
| --- | --- | --- | ---: | ---: |
| ImportSiteAssetsCommand | Data collection from 1-1-2023 till 31-12-2023.xlsx | Yes | 9 | 1832 |
| ImportPowerDataCommand | Data collection from 1-1-2023 till 31-12-2023.xlsx | Yes | 9 | 1830 |
| ImportSiteRadioDataCommand | Data collection from 1-1-2023 till 31-12-2023.xlsx | Yes | 103 | 22079 |
| ImportSiteTxDataCommand | Data collection from 1-1-2023 till 31-12-2023.xlsx | Yes | 27 | 4636 |
| ImportSiteSharingDataCommand | Data collection from 1-1-2023 till 31-12-2023.xlsx | Yes | 1 | 231 |
| ImportRFStatusCommand | GH-DE Data Collection.xlsx | Yes | 0 | 0 |
| ImportBatteryDischargeTestCommand | GH-BDT_BDT.xlsx | Yes | 0 | 1 |
| ImportDeltaSitesCommand | Delta Sites (1).xlsx | Yes | 20 | 3932 |
| ImportChecklistTemplateCommand | GH-DE  Checklist.xlsx | Yes | 259 | 0 |
| ImportPanoramaEvidenceCommand | GH-DE  Checklist.xlsx | Yes | 147 | 0 |
| ImportAlarmCaptureCommand | GH-DE  Checklist.xlsx | Yes | 0 | 1 |

## Top Error Reasons
- Row 6: site '1009DE' not found. (x2)
- Row 7: site '4175DE' not found. (x2)
- Row 2: Site code is missing. (x1)
- Row 3: Site code is missing. (x1)
- Row 8: site '0298DE' not found. (x1)
- Row 10: site '0281DE' not found. (x1)
- Row 11: site '0699DE' not found. (x1)
- Row 12: site '0627DE' not found. (x1)
- Row 13: site '0696DE' not found. (x1)
- Row 14: site '3161CA' not found. (x1)
- Row 4: site '1009DE' not found. (x1)
- Row 5: site '4175DE' not found. (x1)
- Row 6: site '0298DE' not found. (x1)
- Row 8: site '0281DE' not found. (x1)
- Row 9: site '0699DE' not found. (x1)

## Entity-Level Diff
- Sites changed: 8
- Sites with new power system state: 8
- Sites with new transmission state: 8
- Total MW links added: 27
- Sites with new radio state: 7
- Total sector records added: 103
- Sites with new sharing state: 1
- Total sharing antenna positions added: 5
- Sites with RF status changes: 0
- Sites with operational zone changes: 8
- Checklist templates created: 1
- Checklist items imported: 259
- Battery discharge tests imported: 0
- Visit evidence photos after dry-run: 147
