# Phase 2 Observability Runbook

Date: 2026-02-24

## 1) Purpose

This runbook defines what to monitor for the highest-risk operational flows:
- Import pipelines
- Offline sync processing
- Notification delivery (SMS / push / email)

It also defines alert thresholds and the first response actions.

## 2) Correlation and Logging

## Correlation ID contract
- Header: `X-Correlation-ID`
- If client sends the header, API preserves it.
- If missing, API generates one and returns it in response.
- All request logs include correlation ID.

## Log path
- API log sink: `logs/api-log-*.txt` (Serilog file sink)
- App log sink: `logs/towerops-*.txt`

## Security-sensitive flows to inspect with correlation
- Auth endpoints: `/api/auth/*`
- Sync endpoint: `/api/sync`
- Import endpoints: `/api/sites/import*`, `/api/visits/*/import/*`, `/api/checklisttemplates/import`

## 3) Metrics Surface

Metrics are emitted through `System.Diagnostics.Metrics` meter:
- Meter name: `TowerOps.Operations`

## Import metrics
- `towerops_import_requests_total` (counter)
  - tags: `operation`, `outcome`
- `towerops_import_rows_imported_total` (counter)
- `towerops_import_rows_skipped_total` (counter)
- `towerops_import_rows_errors_total` (counter)
- `towerops_import_duration_ms` (histogram)

## Sync metrics
- `towerops_sync_batches_total` (counter)
  - tags: `operation`, `outcome`
- `towerops_sync_items_processed_total` (counter)
- `towerops_sync_items_conflicts_total` (counter)
- `towerops_sync_items_failed_total` (counter)
- `towerops_sync_items_skipped_total` (counter)
- `towerops_sync_batch_duration_ms` (histogram)

## Notification metrics
- `towerops_notifications_total` (counter)
  - tags: `channel`, `outcome`
- `towerops_notification_duration_ms` (histogram)

Channels:
- `sms`
- `email`
- `push`
- `push.signalr`
- `push.firebase`

Outcomes:
- `success`
- `failed`
- `skipped`
- `exception` (import/sync behavior paths)

## 4) Alert Thresholds

## A. Import reliability
- Condition:
  - `outcome=failed|exception` rate > 5% over 15 minutes
  - OR p95 `towerops_import_duration_ms` > 10,000 ms over 15 minutes
- Severity:
  - Warning at 5%
  - Critical at 10%

## B. Sync conflict/failure ratio
- Condition:
  - (`conflicts + failed`) / (`processed + conflicts + failed`) > 10% over 30 minutes
- Severity:
  - Warning at 10%
  - Critical at 20%

## C. Notification delivery
- Condition:
  - `outcome=failed` > 2% over 15 minutes for `sms`, `push.signalr`, or `push.firebase`
- Severity:
  - Warning at 2%
  - Critical at 5%

## D. Notification latency
- Condition:
  - p95 `towerops_notification_duration_ms` > 5,000 ms over 15 minutes
- Severity:
  - Warning

## 5) First Response Playbook

## Import failures
1. Check recent deploy/config changes for `Import:*` settings.
2. Query top failing operation via `operation` tag.
3. Use `X-Correlation-ID` from failing API responses to trace full request.
4. Validate workbook format and row limits.

## Sync failures/conflicts
1. Confirm `Sync:MaxBatchSize`, `Sync:MaxRetries`, and conflict mode settings.
2. Inspect latest conflict reasons (`VisitAlreadySubmitted`, `StaleData`, duplicate payload).
3. Identify client device pattern (`deviceId`, `engineerId`) and isolate if needed.

## Notification failures
1. Verify dynamic settings values for Twilio/Firebase/SMTP.
2. Confirm outbound endpoint reachability and credentials.
3. Check circuit-breaker open states and retry exhaustion in logs.

## 6) Assumptions and Limits

- Metrics are emitted by backend services; dashboard/export wiring is platform-dependent.
- Alerting backend (Prometheus/Grafana/Azure Monitor/etc.) is expected to scrape/forward meter data.
- This runbook does not replace incident postmortem requirements.

## 7) Related Baseline

- Performance thresholds and latest measured results are documented in `docs/phase-2/14-performance-baseline.md`.
