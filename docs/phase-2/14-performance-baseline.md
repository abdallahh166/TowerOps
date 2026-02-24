# Phase 2 Performance and Capacity Baseline

Date: 2026-02-24  
Owner: QA / Platform / Backend

## 1) Purpose

This document records the baseline thresholds and latest measured results for the critical P2-03 performance flows:
- Portal query path
- Site import pipeline
- Offline sync batch processing

These thresholds are enforced by automated tests in `tests/TelecomPM.Application.Tests/Performance/PerformanceBaselineSmokeTests.cs`.

## 2) Baseline Scenarios and Thresholds

| Flow | Test | Dataset / Iterations | Threshold |
|---|---|---|---|
| Portal sites read path | `Baseline_PortalSitesQuery_ShouldCompleteWithinThreshold` | 100 query iterations, page size 200 | `< 3000 ms` |
| Site import pipeline | `Baseline_ImportSiteData_500Rows_ShouldCompleteWithinThreshold` | 500 workbook rows | `< 8000 ms` |
| Offline sync processing | `Baseline_SyncBatch50Items_ShouldCompleteWithinThreshold` | 50 sync items | `< 3000 ms` |

## 3) Latest Measured Run

Command:

```powershell
dotnet test tests/TelecomPM.Application.Tests/TelecomPM.Application.Tests.csproj --configuration Debug --filter "FullyQualifiedName~PerformanceBaselineSmokeTests" --logger "console;verbosity=normal"
```

Observed results:
- Import baseline test: `3 s`
- Portal baseline test: `64 ms`
- Sync baseline test: `42 ms`
- Summary: `Passed 3/3`

## 4) Release Gate Rule

- The performance baseline is considered healthy only when the three smoke tests pass.
- Any threshold breach fails the test and blocks the release pipeline until investigated and re-baselined with evidence.

## 5) Related Evidence

- `tests/TelecomPM.Application.Tests/Performance/PerformanceBaselineSmokeTests.cs`
- `docs/phase-2/12-production-remediation-plan.md`
- `docs/phase-2/13-observability-runbook.md`
