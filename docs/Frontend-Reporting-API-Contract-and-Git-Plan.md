# TowerOps Frontend Reporting, API Contract, Sprint Plan, and Git Strategy

## Purpose
This is the execution document for frontend and backend teams to deliver a production-grade TowerOps UI with reporting, charts, tables, exports, and disciplined Git operations.

Scope of this document:
- Full reporting catalog (implemented + missing professional-grade reports).
- Frontend-ready API contract by endpoint.
- Data contract conventions for React.
- Sprint-by-sprint implementation plan.
- SaaS-grade Git and GitHub workflow.

Assumptions:
- Backend source of truth is `src/TowerOps.Api/Controllers` and `src/TowerOps.Application/*`.
- Authentication is JWT bearer.
- All business timestamps are UTC.

---

## 1) Full Reporting List

### 1.1 Dashboard KPIs (Primary Operations Dashboard)
Endpoint: `GET /api/kpi/operations`

| KPI | Field | Formula / Logic | Filters | Best UI |
|---|---|---|---|---|
| Total Work Orders | `totalWorkOrders` | Count of WOs in filter scope | `fromDateUtc,toDateUtc,officeCode,slaClass` | Big number |
| Open Work Orders | `openWorkOrders` | Non-closed/canceled WOs | Same | Big number + trend |
| Breached Work Orders | `breachedWorkOrders` | SLA-breached WOs | Same | Alert card |
| At-Risk Work Orders | `atRiskWorkOrders` | Near SLA breach | Same | Alert card |
| SLA Compliance % | `slaCompliancePercentage` | On-time / total * 100 | Same | Gauge + trend |
| Total Reviewed Visits | `totalReviewedVisits` | Visits with review outcome | Same | Big number |
| First Time Fix % | `ftfRatePercent` | Closed WOs with no rework/reopen history / closed WOs * 100 | Same | Gauge |
| MTTR (hours) | `mttrHours` | Average(ClosedAt - CreatedAt) in hours | Same | KPI + trend line |
| Reopen Rate % | `reopenRatePercent` | Reopened WOs / closed WOs * 100 | Same | Gauge |
| Evidence Completeness % | `evidenceCompletenessPercent` | Visits meeting evidence policy / submitted visits * 100 | Same | Gauge |

Backward-compatible alias fields also exist:
- `firstTimeFixPercentage`
- `meanTimeToRepairHours`
- `reopenRatePercentage`
- `evidenceCompletenessPercentage`

### 1.2 Statistical / Chart Endpoints

| Endpoint | Primary Data | Recommended Chart |
|---|---|---|
| `GET /api/analytics/visit-completion-trends` | `VisitCompletionTrendDto[]` by period | Multi-line (completion, approval), stacked columns (status counts) |
| `GET /api/analytics/issue-analytics` | Severity/category/site distribution + resolution KPIs | Donut + stacked bar + top table |
| `GET /api/analytics/engineer-performance/{engineerId}` | Engineer productivity and quality | Radar + scorecards + trend |
| `GET /api/analytics/site-maintenance/{siteId}` | Site maintenance depth (issues/materials/history) | Timeline + stacked bar |
| `GET /api/analytics/office-statistics/{officeId}` | Office-level operations distribution | Dashboard cards + grouped bars |
| `GET /api/analytics/material-usage/{materialId}` | Material consumption and cost | Bar + cumulative line |
| `GET /api/kpi/operations` | Operations KPI snapshot | KPI grid + gauges |
| `GET /api/portal/sla-report` | Monthly SLA by class for client portal | Small multiples line/bar |
| `GET /api/portal/dashboard` | Client-facing headline KPIs | Compact KPI cards |

### 1.3 Tabular and Filterable Reports

| Report | Endpoint | Filters | Table Notes |
|---|---|---|---|
| Engineer Visit Queue | `GET /api/visits/engineers/{engineerId}` | `status,from,to,pageNumber,pageSize` | Paginated |
| Pending Reviews | `GET /api/visits/pending-reviews` | `officeId` | Operational review queue |
| Scheduled Visits | `GET /api/visits/scheduled` | `date,engineerId` | Daily scheduling grid |
| Office Sites | `GET /api/sites/office/{officeId}` | `pageNumber,pageSize,complexity,status` | Paginated |
| Maintenance Needed Sites | `GET /api/sites/maintenance` | `daysThreshold,officeId` | Prioritized backlog |
| Materials by Office | `GET /api/materials` | `officeId,onlyInStock` | Stock list |
| Low Stock Materials | `GET /api/materials/low-stock/{officeId}` | none | Procurement trigger list |
| Users by Office | `GET /api/users/office/{officeId}` | none | Staff coverage |
| Users by Role | `GET /api/users/role/{role}` | none | Role governance |
| Portal Sites | `GET /api/portal/sites` | `pageNumber,pageSize` | Client-facing list |
| Portal Work Orders | `GET /api/portal/workorders` | `pageNumber,pageSize` | Client-facing WO table |
| Portal Visits by Site | `GET /api/portal/visits/{siteCode}` | `pageNumber,pageSize` | Client-facing visit history |
| Sync Status | `GET /api/sync/status/{deviceId}` | none | Offline sync telemetry |
| Sync Conflicts | `GET /api/sync/conflicts/{engineerId}` | none | Conflict resolution queue |
| Assets by Site | `GET /api/assets/site/{siteCode}` | none | Asset inventory view |
| Faulty Assets | `GET /api/assets/faulty` | none | Reliability ops |
| Expiring Warranties | `GET /api/assets/expiring-warranties` | `days` | Preventive alerts |

### 1.4 Operational / Export Reports

| Report | Endpoint | Type |
|---|---|---|
| Visit Evidence Report | `GET /api/reports/visits/{visitId}` | JSON |
| Monthly Contractor Scorecard | `GET /api/reports/scorecard?officeCode&month&year` | Excel file |
| Checklist Export | `GET /api/reports/checklist?visitId&visitType` | Excel file |
| BDT Export | `GET /api/reports/bdt?fromDateUtc&toDateUtc` | Excel file |
| Data Collection Export | `GET /api/reports/data-collection?officeCode` | Excel file |

### 1.5 Performance Metrics for Professional Operations

Already implemented:
- FTF, MTTR, Reopen Rate, Evidence Completeness (KPI endpoint).
- SLA compliance and breach indicators.
- Engineer and office performance analytics.
- Material usage and stock risk indicators.

Should be added for professional maturity:
- SLA breach root-cause taxonomy trend.
- Aging buckets (WO/Visit/Escalation) with SLA clocks.
- Rework loop depth and correction-cycle ratio.
- Import quality trend (imported/skipped/errors by template and by office).
- Sync health trend (pending/failed/conflicts over time).
- Notification delivery report (SMS/email/push success/failure latency).
- Forecast reports (materials reorder forecast, engineer capacity forecast).

### 1.6 Missing Reports (Recommended Backlog)

P0 (must have before commercial rollout):
- SLA breach root-cause and owner accountability report.
- Backlog aging report (WO + Visits + Escalations with SLA buckets).
- Import quality report with per-template error taxonomy.

P1:
- Sync health report (device reliability and retry trend).
- Notification delivery and latency report.
- Engineer utilization heatmap by office/day.

P2:
- Predictive maintenance risk scoring.
- Capacity forecast and recommended staffing.

---

## 2) Complete API Contract (Frontend-Focused)

## 2.1 Global API Conventions
- Base path: `/api/*`
- Auth header: `Authorization: Bearer <jwt>`
- Correlation header: `X-Correlation-ID` (returned by server for traceability)
- Localization: query/cookie/`Accept-Language` (`en-US`, `ar-EG`)
- Content types:
  - JSON: `application/json`
  - File upload: `multipart/form-data`
  - Excel exports: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`

### 2.2 Current Success Response Contract (Implemented)
- Most endpoints return raw payload objects/arrays on `200`.
- Command endpoints with no payload return `200` with empty body.
- Create endpoints commonly return `201 Created` with body payload.

### 2.3 Standard Error Contract (Implemented)
```json
{
  "code": "request.validation_failed",
  "message": "Validation failed",
  "correlationId": "0HNJKFGJTL6GU:00000004",
  "errors": {
    "email": ["Email is required"]
  },
  "meta": {
    "Rule": "WorkOrderScopeRule"
  }
}
```

Error code set:
- `internal.error`
- `request.failed`
- `request.validation_failed`
- `resource.not_found`
- `auth.unauthorized`
- `auth.forbidden`
- `request.conflict`
- `business.rule_violation`

### 2.4 Pagination Contract

Implemented paginated shape (`PaginatedList<T>`):
```json
{
  "items": [],
  "pageNumber": 1,
  "totalPages": 10,
  "totalCount": 192,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

Implemented pagination query format:
- `?pageNumber=1&pageSize=20`
- max page size is commonly capped at 200 in handlers/controllers.

Some endpoints accept page params but return arrays without `totalCount` (not ideal). Frontend should treat these as server-side slice responses.

### 2.5 Filtering Contract
Date filters:
- `fromDateUtc`, `toDateUtc` (UTC ISO-8601 preferred)
- Some analytics endpoints use `fromDate`, `toDate` (still UTC in practice)

Common filter examples:
- `GET /api/kpi/operations?fromDateUtc=2026-01-01T00:00:00Z&toDateUtc=2026-01-31T23:59:59Z&officeCode=DE&slaClass=P1`
- `GET /api/analytics/visit-completion-trends?officeId=<guid>&period=Monthly&fromDate=2026-01-01&toDate=2026-01-31`
- `GET /api/sites/office/<officeId>?pageNumber=1&pageSize=20&status=Active`

---

## 2.6 Endpoint-by-Endpoint Contract Registry

Note:
- Request DTO names are source DTOs in `src/TowerOps.Api/Contracts/*`.
- Response DTO names are application DTOs in `src/TowerOps.Application/DTOs/*`.
- `Empty` means `200 OK` with no body.

### Auth (`/api/auth`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| POST | `/api/auth/login` | `LoginRequest` | `LoginResponse` | `EX-01` |
| POST | `/api/auth/forgot-password` | `ForgotPasswordRequest` | `{ message: string }` | `EX-22` |
| POST | `/api/auth/reset-password` | `ResetPasswordRequest` | `{ message: string }` | `EX-22` |
| POST | `/api/auth/change-password` | `ChangePasswordRequest` | `{ message: string }` | `EX-22` |

### KPI (`/api/kpi`)
| Method | Route | Request DTO | Response DTO | Filters | Example |
|---|---|---|---|---|---|
| GET | `/api/kpi/operations` | Query params | `OperationsKpiDashboardDto` | `fromDateUtc,toDateUtc,officeCode,slaClass` | `EX-02` |

### Analytics (`/api/analytics`)
| Method | Route | Request DTO | Response DTO | Filters | Example |
|---|---|---|---|---|---|
| GET | `/api/analytics/engineer-performance/{engineerId}` | Query params | `EngineerPerformanceReportDto` | `fromDate,toDate` | `EX-23` |
| GET | `/api/analytics/site-maintenance/{siteId}` | Query params | `SiteMaintenanceReportDto` | `fromDate,toDate` | `EX-24` |
| GET | `/api/analytics/office-statistics/{officeId}` | Query params | `OfficeStatisticsReportDto` | `fromDate,toDate` | `EX-25` |
| GET | `/api/analytics/material-usage/{materialId}` | Query params | `MaterialUsageReportDto` | `fromDate,toDate` | `EX-26` |
| GET | `/api/analytics/visit-completion-trends` | Query params | `VisitCompletionTrendDto[]` | `officeId,engineerId,fromDate,toDate,period` | `EX-27` |
| GET | `/api/analytics/issue-analytics` | Query params | `IssueAnalyticsReportDto` | `officeId,siteId,fromDate,toDate` | `EX-28` |

### Reports (`/api/reports`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| GET | `/api/reports/visits/{visitId}` | Route param | `VisitReportDto` | `EX-29` |
| GET | `/api/reports/scorecard` | Query params | Excel file bytes | `EX-FILE-01` |
| GET | `/api/reports/checklist` | Query params | Excel file bytes | `EX-FILE-02` |
| GET | `/api/reports/bdt` | Query params | Excel file bytes | `EX-FILE-03` |
| GET | `/api/reports/data-collection` | Query params | Excel file bytes | `EX-FILE-04` |

### Client Portal (`/api/portal`)
| Method | Route | Request DTO | Response DTO | Pagination/Filters | Example |
|---|---|---|---|---|---|
| GET | `/api/portal/dashboard` | - | `PortalDashboardDto` | none | `EX-12` |
| GET | `/api/portal/sites` | Query params | `PortalSiteDto[]` | `pageNumber,pageSize` | `EX-30` |
| GET | `/api/portal/sites/{siteCode}` | Route param | `PortalSiteDto` | none | `EX-31` |
| GET | `/api/portal/workorders` | Query params | `PortalWorkOrderDto[]` | `pageNumber,pageSize` | `EX-32` |
| GET | `/api/portal/sla-report` | - | `PortalSlaReportDto` | none | `EX-13` |
| GET | `/api/portal/visits/{siteCode}` | Route + query | `PortalVisitDto[]` | `pageNumber,pageSize` | `EX-33` |
| GET | `/api/portal/visits/{visitId}/evidence` | Route param | `PortalVisitEvidenceDto` | none | `EX-34` |
| PATCH | `/api/portal/workorders/{id}/accept` | - | `WorkOrderDto` | none | `EX-07` |
| PATCH | `/api/portal/workorders/{id}/reject` | `PortalRejectWorkOrderRequest` | `WorkOrderDto` | none | `EX-07` |

### Visits (`/api/visits`)
| Method | Route | Request DTO | Response DTO | Pagination/Filters | Example |
|---|---|---|---|---|---|
| GET | `/api/visits/{visitId}` | Route param | `VisitDetailDto` | none | `EX-35` |
| GET | `/api/visits/engineers/{engineerId}` | `EngineerVisitQueryParameters` | `PaginatedList<VisitDto>` | `pageNumber,pageSize,status,from,to` | `EX-04` |
| GET | `/api/visits/pending-reviews` | Query params | `VisitDto[]` | `officeId` | `EX-36` |
| GET | `/api/visits/scheduled` | `ScheduledVisitsQueryParameters` | `VisitDto[]` | `date,engineerId` | `EX-37` |
| POST | `/api/visits` | `CreateVisitRequest` | `VisitDto` (`201`) | none | `EX-03` |
| GET | `/api/visits/{visitId}/evidence-status` | Route param | `VisitEvidenceStatusDto` | none | `EX-38` |
| POST | `/api/visits/{visitId}/start` | `StartVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/checkin` | `CheckInVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/checkout` | `CheckOutVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/complete` | `CompleteVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/submit` | - | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/approve` | `ApproveVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/reject` | `RejectVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/request-correction` | `RequestCorrectionRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/checklist-items` | `AddChecklistItemRequest` | `VisitChecklistDto` | none | `EX-39` |
| PATCH | `/api/visits/{visitId}/checklist-items/{checklistItemId}` | `UpdateChecklistItemRequest` | `VisitChecklistDto` | none | `EX-39` |
| POST | `/api/visits/{visitId}/issues` | `AddVisitIssueRequest` | `VisitIssueDto` | none | `EX-40` |
| POST | `/api/visits/{visitId}/issues/{issueId}/resolve` | `ResolveVisitIssueRequest` | `VisitIssueDto` | none | `EX-40` |
| POST | `/api/visits/{visitId}/readings` | `AddVisitReadingRequest` | `VisitReadingDto` | none | `EX-41` |
| PATCH | `/api/visits/{visitId}/readings/{readingId}` | `UpdateVisitReadingRequest` | `VisitReadingDto` | none | `EX-41` |
| POST | `/api/visits/{visitId}/photos` | `AddVisitPhotoRequest` (`multipart/form-data`) | `VisitPhotoDto` | none | `EX-42` |
| DELETE | `/api/visits/{visitId}/photos/{photoId}` | Route params | `Empty` | none | `EX-21` |
| POST | `/api/visits/{visitId}/cancel` | `CancelVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/reschedule` | `RescheduleVisitRequest` | `VisitDto` | none | `EX-03` |
| POST | `/api/visits/{visitId}/signature` | `CaptureVisitSignatureRequest` | `SignatureDto` | none | `EX-19` |
| GET | `/api/visits/{visitId}/signature` | Route param | `SignatureDto` | none | `EX-19` |
| POST | `/api/visits/{visitId}/import/panorama` | `ImportVisitEvidenceRequest` (`multipart/form-data`) | `ImportSiteDataResult` | none | `EX-06` |
| POST | `/api/visits/{visitId}/import/alarms` | `ImportVisitEvidenceRequest` (`multipart/form-data`) | `ImportSiteDataResult` | none | `EX-06` |
| POST | `/api/visits/{visitId}/import/unused-assets` | `ImportVisitEvidenceRequest` (`multipart/form-data`) | `ImportSiteDataResult` | none | `EX-06` |

### Work Orders (`/api/workorders`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| POST | `/api/workorders` | `CreateWorkOrderRequest` | `WorkOrderDto` (`201`) | `EX-07` |
| GET | `/api/workorders/{workOrderId}` | Route param | `WorkOrderDto` | `EX-07` |
| POST | `/api/workorders/{workOrderId}/assign` | `AssignWorkOrderRequest` | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/start` | - | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/complete` | - | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/close` | - | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/cancel` | - | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/submit-for-acceptance` | - | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/customer-accept` | `CustomerAcceptWorkOrderRequest` | `WorkOrderDto` | `EX-07` |
| PATCH | `/api/workorders/{id}/customer-reject` | `CustomerRejectWorkOrderRequest` | `WorkOrderDto` | `EX-07` |
| POST | `/api/workorders/{id}/signature` | `CaptureWorkOrderSignatureRequest` | `WorkOrderSignaturesDto` | `EX-43` |
| GET | `/api/workorders/{id}/signature` | Route param | `WorkOrderSignaturesDto` | `EX-43` |

### Escalations (`/api/escalations`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| POST | `/api/escalations` | `CreateEscalationRequest` | `EscalationDto` (`201`) | `EX-08` |
| GET | `/api/escalations/{escalationId}` | Route param | `EscalationDto` | `EX-08` |
| PATCH | `/api/escalations/{id}/review` | - | `EscalationDto` | `EX-08` |
| PATCH | `/api/escalations/{id}/approve` | - | `EscalationDto` | `EX-08` |
| PATCH | `/api/escalations/{id}/reject` | - | `EscalationDto` | `EX-08` |
| PATCH | `/api/escalations/{id}/close` | - | `EscalationDto` | `EX-08` |

### Sites (`/api/sites`)
| Method | Route | Request DTO | Response DTO | Pagination/Filters | Example |
|---|---|---|---|---|---|
| POST | `/api/sites` | `CreateSiteRequest` | `SiteDetailDto` (`201`) | - | `EX-44` |
| PUT | `/api/sites/{siteId}` | `UpdateSiteRequest` | `SiteDetailDto` | - | `EX-44` |
| PATCH | `/api/sites/{siteId}/status` | `UpdateSiteStatusRequest` | `SiteDto` | - | `EX-05` |
| POST | `/api/sites/{siteId}/assign` | `AssignEngineerRequest` | `SiteDto` | - | `EX-05` |
| POST | `/api/sites/{siteId}/unassign` | - | `SiteDto` | - | `EX-05` |
| PUT | `/api/sites/{siteCode}/ownership` | `UpdateSiteOwnershipRequest` | `SiteDetailDto` | - | `EX-44` |
| GET | `/api/sites/{siteId}` | Route param | `SiteDetailDto` | - | `EX-44` |
| GET | `/api/sites/{siteCode}/location` | Route param | `SiteLocationDto` | - | `EX-45` |
| GET | `/api/sites/office/{officeId}` | `OfficeSitesQueryParameters` | `PaginatedList<SiteDto>` | `pageNumber,pageSize,complexity,status` | `EX-46` |
| GET | `/api/sites/maintenance` | `MaintenanceSitesQueryParameters` | `SiteDto[]` | `daysThreshold,officeId` | `EX-47` |
| POST | `/api/sites/import` | `ImportSiteDataRequest` (`multipart/form-data`) | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/site-assets` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/power-data` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/radio-data` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/tx-data` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/sharing-data` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/rf-status` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/battery-discharge-tests` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |
| POST | `/api/sites/import/delta-sites` | `ImportSiteDataRequest` | `ImportSiteDataResult` | - | `EX-06` |

### Materials (`/api/materials`)
| Method | Route | Request DTO | Response DTO | Filters | Example |
|---|---|---|---|---|---|
| POST | `/api/materials` | `CreateMaterialRequest` | `MaterialDto` (`201`) | - | `EX-09` |
| PUT | `/api/materials/{id}` | `UpdateMaterialRequest` | `MaterialDto` | - | `EX-09` |
| DELETE | `/api/materials/{id}` | Route param | `Empty` | - | `EX-21` |
| POST | `/api/materials/{id}/stock/add` | `AddStockRequest` | `MaterialDto` | - | `EX-09` |
| POST | `/api/materials/{id}/stock/reserve` | `ReserveStockRequest` | `MaterialDto` | - | `EX-09` |
| POST | `/api/materials/{id}/stock/consume` | `ConsumeStockRequest` | `MaterialDto` | - | `EX-09` |
| GET | `/api/materials/{id}` | Route param | `MaterialDetailDto` | - | `EX-48` |
| GET | `/api/materials` | Query params | `MaterialDto[]` | `officeId,onlyInStock` | `EX-49` |
| GET | `/api/materials/low-stock/{officeId}` | Route param | `MaterialDto[]` | - | `EX-49` |

### Users (`/api/users`)
| Method | Route | Request DTO | Response DTO | Filters | Example |
|---|---|---|---|---|---|
| POST | `/api/users` | `CreateUserRequest` | `UserDto` (`201`) | - | `EX-11` |
| GET | `/api/users/{userId}` | Route param | `UserDetailDto` | - | `EX-50` |
| PUT | `/api/users/{userId}` | `UpdateUserRequest` | `UserDto` | - | `EX-11` |
| DELETE | `/api/users/{userId}` | Route param | `Empty` | - | `EX-21` |
| PATCH | `/api/users/{userId}/role` | `ChangeUserRoleRequest` | `UserDto` | - | `EX-11` |
| PATCH | `/api/users/{userId}/activate` | - | `UserDto` | - | `EX-11` |
| PATCH | `/api/users/{userId}/deactivate` | - | `UserDto` | - | `EX-11` |
| GET | `/api/users/office/{officeId}` | Route param | `UserDto[]` | - | `EX-51` |
| GET | `/api/users/role/{role}` | Route param | `UserDto[]` | - | `EX-51` |
| GET | `/api/users/{userId}/performance` | Query params | `UserPerformanceDto` | `fromDate,toDate` | `EX-52` |

### Offices (`/api/offices`)
| Method | Route | Request DTO | Response DTO | Filters/Pagination | Example |
|---|---|---|---|---|---|
| POST | `/api/offices` | `CreateOfficeRequest` | `OfficeDto` (`201`) | - | `EX-10` |
| GET | `/api/offices/{officeId}` | Route param | `OfficeDetailDto` | - | `EX-53` |
| GET | `/api/offices` | Query params | `OfficeDto[]` | `onlyActive,pageNumber,pageSize` | `EX-54` |
| GET | `/api/offices/region/{region}` | Route param | `OfficeDto[]` | - | `EX-54` |
| GET | `/api/offices/{officeId}/statistics` | Route param | `OfficeStatisticsDto` | - | `EX-55` |
| PUT | `/api/offices/{officeId}` | `UpdateOfficeRequest` | `OfficeDto` | - | `EX-10` |
| PATCH | `/api/offices/{officeId}/contact` | `UpdateOfficeContactRequest` | `OfficeDto` | - | `EX-10` |
| DELETE | `/api/offices/{officeId}` | Route param | `Empty` | - | `EX-21` |

### Settings (`/api/settings`)
| Method | Route | Request DTO | Response DTO | Pagination | Example |
|---|---|---|---|---|---|
| GET | `/api/settings` | Query params | `Dictionary<string, SystemSettingResponse[]>` | `pageNumber,pageSize` | `EX-17` |
| GET | `/api/settings/{group}` | Route param | `SystemSettingResponse[]` | - | `EX-56` |
| PUT | `/api/settings` | `UpsertSystemSettingRequest[]` | `Empty` | - | `EX-21` |
| POST | `/api/settings/test/{service}` | Route param | `{ message: string }` | - | `EX-22` |

### Roles (`/api/roles`)
| Method | Route | Request DTO | Response DTO | Pagination | Example |
|---|---|---|---|---|---|
| GET | `/api/roles` | Query params | `RoleResponse[]` | `pageNumber,pageSize` | `EX-18` |
| GET | `/api/roles/permissions` | - | `string[]` | - | `EX-57` |
| GET | `/api/roles/{id}` | Route param | `RoleResponse` | - | `EX-18` |
| POST | `/api/roles` | `CreateRoleRequest` | `RoleResponse` (`201`) | - | `EX-18` |
| PUT | `/api/roles/{id}` | `UpdateRoleRequest` | `RoleResponse` | - | `EX-18` |
| DELETE | `/api/roles/{id}` | Route param | `204 No Content` | - | `EX-58` |

### Daily Plans (`/api/daily-plans`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| POST | `/api/daily-plans` | `CreateDailyPlanRequest` | `DailyPlanDto` | `EX-14` |
| GET | `/api/daily-plans/{officeId}/{date}` | Route params | `DailyPlanDto` | `EX-14` |
| POST | `/api/daily-plans/{planId}/assign` | `AssignSiteToEngineerRequest` | `DailyPlanDto` | `EX-14` |
| DELETE | `/api/daily-plans/{planId}/assign` | `RemoveSiteFromEngineerRequest` | `DailyPlanDto` | `EX-14` |
| GET | `/api/daily-plans/{planId}/suggest/{engineerId}` | Route params | `PlannedVisitStopDto[]` | `EX-59` |
| GET | `/api/daily-plans/{officeId}/{date}/unassigned` | Route params | `UnassignedSiteDto[]` | `EX-60` |
| POST | `/api/daily-plans/{planId}/publish` | - | `DailyPlanDto` | `EX-14` |

### Checklist Templates (`/api/checklisttemplates`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| GET | `/api/checklisttemplates?visitType=BM` | Query params | `ChecklistTemplateDto` | `EX-61` |
| GET | `/api/checklisttemplates/{id}` | Route param | `ChecklistTemplateDto` | `EX-61` |
| GET | `/api/checklisttemplates/history?visitType=BM` | Query params | `ChecklistTemplateDto[]` | `EX-62` |
| POST | `/api/checklisttemplates` | `CreateChecklistTemplateRequest` | `Guid` (`201`) | `EX-63` |
| POST | `/api/checklisttemplates/{id}/activate` | `ActivateChecklistTemplateRequest` | `Empty` | `EX-21` |
| POST | `/api/checklisttemplates/import` | `ImportChecklistTemplateRequest` (`multipart/form-data`) | `ImportSiteDataResult` | `EX-06` |

### Assets (`/api/assets`)
| Method | Route | Request DTO | Response DTO | Filters | Example |
|---|---|---|---|---|---|
| GET | `/api/assets/site/{siteCode}` | Route param | `AssetDto[]` | - | `EX-64` |
| GET | `/api/assets/{assetCode}` | Route param | `AssetDto` | - | `EX-15` |
| GET | `/api/assets/{assetCode}/history` | Route param | `AssetDto` | - | `EX-15` |
| POST | `/api/assets` | `RegisterAssetRequest` | `AssetDto` | - | `EX-15` |
| PUT | `/api/assets/{assetCode}/service` | `RecordAssetServiceRequest` | `AssetDto` | - | `EX-15` |
| PUT | `/api/assets/{assetCode}/fault` | `MarkAssetFaultyRequest` | `AssetDto` | - | `EX-15` |
| PUT | `/api/assets/{assetCode}/replace` | `ReplaceAssetRequest` | `AssetDto` | - | `EX-15` |
| GET | `/api/assets/expiring-warranties` | Query params | `AssetDto[]` | `days` | `EX-64` |
| GET | `/api/assets/faulty` | - | `AssetDto[]` | - | `EX-64` |

### Sync (`/api/sync`)
| Method | Route | Request DTO | Response DTO | Example |
|---|---|---|---|---|
| POST | `/api/sync` | `SyncBatchRequest` | `SyncResultDto` | `EX-16` |
| GET | `/api/sync/status/{deviceId}` | Route param | `SyncStatusDto` | `EX-65` |
| GET | `/api/sync/conflicts/{engineerId}` | Route param | `SyncConflictDto[]` | `EX-66` |

---

## 3) Frontend Data Contract Document (React-Ready)

### 3.1 Recommended Frontend Response Adapter
Because backend success responses are not wrapped consistently, implement a single API adapter in frontend:

```ts
type ApiError = {
  code: string;
  message: string;
  correlationId: string;
  errors?: Record<string, string[]>;
  meta?: Record<string, string>;
};

type ApiSuccess<T> = {
  success: true;
  data: T;
  meta?: {
    pagination?: {
      pageNumber: number;
      totalPages: number;
      totalCount: number;
      hasPreviousPage: boolean;
      hasNextPage: boolean;
    };
    filters?: Record<string, unknown>;
    generatedAtUtc?: string;
  };
};
```

Adapter rule:
- If server returns paginated object (`items + pageNumber + totalPages`), map to `ApiSuccess<T[]>` and put paging under `meta.pagination`.
- If server returns array/object directly, map as `data`.
- If empty `200`, map `data: null`.
- If file response, bypass adapter and handle as blob download.

### 3.2 TypeScript Contract Skeleton

Core report models:
- `OperationsKpiDashboardDto`
- `VisitCompletionTrendDto[]`
- `IssueAnalyticsReportDto`
- `EngineerPerformanceReportDto`
- `SiteMaintenanceReportDto`
- `OfficeStatisticsReportDto`
- `MaterialUsageReportDto`
- `PortalDashboardDto`
- `PortalSlaReportDto`

Core operations models:
- `VisitDto`, `VisitDetailDto`, `VisitEvidenceStatusDto`
- `WorkOrderDto`
- `SiteDto`, `SiteDetailDto`, `SiteLocationDto`
- `EscalationDto`
- `MaterialDto`, `MaterialDetailDto`
- `UserDto`, `UserDetailDto`, `UserPerformanceDto`
- `OfficeDto`, `OfficeDetailDto`, `OfficeStatisticsDto`
- `AssetDto`
- `DailyPlanDto`
- `ImportSiteDataResult`
- `SyncResultDto`, `SyncStatusDto`, `SyncConflictDto`

### 3.3 Example JSON Responses

`EX-01` Login response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAtUtc": "2026-03-10T10:15:00Z",
  "userId": "9fd4ca8f-3b04-4ff3-bb38-67b51fcb9a0e",
  "email": "admin@towerops.com",
  "role": "Admin",
  "officeId": "9af3537a-e213-47f4-bb5e-ddcb95a9f654",
  "requiresPasswordChange": false
}
```

`EX-02` KPI operations:
```json
{
  "generatedAtUtc": "2026-02-25T18:00:00Z",
  "fromDateUtc": "2026-02-01T00:00:00Z",
  "toDateUtc": "2026-02-25T23:59:59Z",
  "officeCode": "DE",
  "slaClass": "P1",
  "totalWorkOrders": 124,
  "openWorkOrders": 22,
  "breachedWorkOrders": 7,
  "atRiskWorkOrders": 5,
  "slaCompliancePercentage": 94.35,
  "totalReviewedVisits": 83,
  "ftfRatePercent": 88.7,
  "mttrHours": 5.2,
  "reopenRatePercent": 6.1,
  "evidenceCompletenessPercent": 91.4
}
```

`EX-04` Paginated visit list:
```json
{
  "items": [
    {
      "id": "8277c3ad-c2de-4406-9730-25e595a4f7fd",
      "visitNumber": "VST-20260225-0012",
      "siteCode": "3564DE",
      "siteName": "DE-GF-3564",
      "engineerName": "Field Engineer",
      "scheduledDate": "2026-02-25T08:00:00Z",
      "status": "InProgress",
      "type": "BM",
      "completionPercentage": 65,
      "canBeEdited": true,
      "canBeSubmitted": false
    }
  ],
  "pageNumber": 1,
  "totalPages": 12,
  "totalCount": 232,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

`EX-06` Import result:
```json
{
  "importedCount": 154,
  "skippedCount": 12,
  "errors": [
    "Row 18: SiteCode invalid format",
    "Row 23: OfficeCode not found"
  ]
}
```

`EX-20` Error (validation):
```json
{
  "code": "request.validation_failed",
  "message": "Validation failed",
  "correlationId": "0HNJKFGJTL6GU:00000004",
  "errors": {
    "siteCode": ["Site code is required"]
  }
}
```

`EX-21` Empty success:
- HTTP `200 OK` with no JSON body.

`EX-22` Message-only success:
```json
{
  "message": "Operation completed successfully."
}
```

`EX-FILE-01..04` File responses:
- HTTP `200`
- content type: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- body: binary file stream
- response header contains download filename.

### 3.4 Additional Response Examples (Referenced IDs)

`EX-23` Engineer performance:
```json
{ "engineerId": "guid", "engineerName": "Engineer 01", "totalVisits": 42, "completionRate": 90.4, "approvalRate": 88.1, "onTimeRate": 86.0 }
```

`EX-24` Site maintenance:
```json
{ "siteId": "guid", "siteCode": "3564DE", "totalVisits": 18, "openIssues": 3, "criticalIssues": 1, "maintenanceHistory": [] }
```

`EX-25` Office statistics report:
```json
{ "officeId": "guid", "officeCode": "DE", "totalSites": 240, "totalVisits": 980, "approvedVisits": 811, "lowStockMaterials": 12 }
```

`EX-26` Material usage report:
```json
{ "officeId": "guid", "fromDate": "2026-02-01T00:00:00Z", "toDate": "2026-02-28T23:59:59Z", "items": [], "totalMaterialsTracked": 44, "totalTransactions": 391 }
```

`EX-27` Visit completion trends:
```json
[{ "period": "2026-02-01T00:00:00Z", "totalVisits": 220, "completedVisits": 205, "approvedVisits": 190, "completionRate": 93.2 }]
```

`EX-28` Issue analytics:
```json
{ "totalIssues": 96, "openIssues": 17, "resolvedIssues": 79, "criticalIssues": 6, "resolutionRate": 82.3, "issuesByCategory": [], "issuesBySeverity": [] }
```

`EX-29` Visit report:
```json
{ "visit": { "id": "guid", "visitNumber": "VST-20260225-0012" }, "site": { "id": "guid", "siteCode": "3564DE" }, "photoComparisons": [], "totalMaterialCost": 1250.0 }
```

`EX-30` Portal site list:
```json
[{ "siteCode": "3564DE", "name": "DE-GF-3564", "status": "Active", "region": "Delta", "openWorkOrdersCount": 2 }]
```

`EX-31` Portal site:
```json
{ "siteCode": "3564DE", "name": "DE-GF-3564", "status": "Active", "region": "Delta", "lastVisitType": "BM" }
```

`EX-32` Portal work orders:
```json
[{ "workOrderId": "guid", "siteCode": "3564DE", "status": "Assigned", "priority": "P2", "slaDeadline": "2026-02-26T07:00:00Z" }]
```

`EX-33` Portal visits:
```json
[{ "visitId": "guid", "visitNumber": "VST-20260225-0012", "status": "Approved", "type": "BM", "scheduledDate": "2026-02-25T08:00:00Z", "engineerDisplayName": "Field Engineer" }]
```

`EX-34` Portal visit evidence:
```json
{ "visitId": "guid", "visitNumber": "VST-20260225-0012", "siteCode": "3564DE", "photos": [], "readings": [], "checklistItems": [] }
```

`EX-35` Visit detail:
```json
{ "id": "guid", "visitNumber": "VST-20260225-0012", "photos": [], "readings": [], "checklists": [], "issuesFound": [], "approvalHistory": [] }
```

`EX-36` Pending reviews:
```json
[{ "id": "guid", "visitNumber": "VST-20260225-0012", "status": "Submitted", "type": "BM", "siteCode": "3564DE" }]
```

`EX-37` Scheduled visits:
```json
[{ "id": "guid", "visitNumber": "VST-20260225-0013", "status": "Scheduled", "type": "CM", "siteCode": "4411DE" }]
```

`EX-38` Visit evidence status:
```json
{ "visitId": "guid", "beforePhotos": 3, "afterPhotos": 2, "requiredBeforePhotos": 3, "requiredAfterPhotos": 2, "readingsCount": 14, "requiredReadings": 15, "completionPercentage": 92, "canBeSubmitted": false }
```

`EX-39` Checklist item:
```json
{ "id": "guid", "category": "Power", "itemName": "Rectifier Visual Check", "status": "OK", "isMandatory": true, "notes": "No alarm" }
```

`EX-40` Visit issue:
```json
{ "id": "guid", "category": "Power", "severity": "High", "title": "Battery low", "status": "Open", "reportedAt": "2026-02-25T08:10:00Z" }
```

`EX-41` Visit reading:
```json
{ "id": "guid", "readingType": "BatteryVoltage", "category": "Power", "value": 53.4, "unit": "V", "isWithinRange": true, "measuredAt": "2026-02-25T08:20:00Z" }
```

`EX-42` Visit photo:
```json
{ "id": "guid", "type": "After", "category": "Panorama", "itemName": "Site Front", "fileUrl": "https://...", "capturedAt": "2026-02-25T08:22:00Z" }
```

`EX-43` Work order signatures:
```json
{ "clientSignature": { "signerName": "Client Rep", "signedAtUtc": "2026-02-25T12:10:00Z" }, "engineerSignature": { "signerName": "Engineer 01", "signedAtUtc": "2026-02-25T12:00:00Z" } }
```

`EX-44` Site detail:
```json
{ "id": "guid", "siteCode": "3564DE", "name": "DE-GF-3564", "region": "Delta", "siteType": "GreenField", "status": "Active", "towerOwnershipType": "Host" }
```

`EX-45` Site location:
```json
{ "siteCode": "3564DE", "latitude": 31.205, "longitude": 30.021, "allowedRadiusMeters": 200 }
```

`EX-46` Paginated office sites:
```json
{ "items": [{ "id": "guid", "siteCode": "3564DE", "name": "DE-GF-3564", "status": "Active" }], "pageNumber": 1, "totalPages": 3, "totalCount": 56, "hasPreviousPage": false, "hasNextPage": true }
```

`EX-47` Maintenance sites:
```json
[{ "id": "guid", "siteCode": "4411DE", "name": "DE-RT-4411", "lastVisitDate": "2025-12-31T00:00:00Z", "status": "Active" }]
```

`EX-48` Material detail:
```json
{ "id": "guid", "code": "MAT-001", "name": "Battery 12V", "currentStock": 34, "minimumStock": 20, "recentTransactions": [], "activeReservations": [] }
```

`EX-49` Materials list:
```json
[{ "id": "guid", "code": "MAT-001", "name": "Battery 12V", "currentStock": 34, "isLowStock": false }]
```

`EX-50` User detail:
```json
{ "id": "guid", "name": "Engineer 01", "email": "eng01@towerops.com", "role": "Engineer", "specializations": ["Power"], "assignedSiteIds": [] }
```

`EX-51` Users list:
```json
[{ "id": "guid", "name": "Engineer 01", "email": "eng01@towerops.com", "role": "Engineer", "officeName": "Delta Office", "isActive": true }]
```

`EX-52` User performance:
```json
{ "userId": "guid", "userName": "Engineer 01", "totalVisits": 52, "completedVisits": 47, "approvalRate": 90.2, "onTimeRate": 88.0 }
```

`EX-53` Office detail:
```json
{ "id": "guid", "code": "DE", "name": "Delta Office", "region": "Delta", "city": "Mansoura", "contactPerson": "Office Manager" }
```

`EX-54` Office list:
```json
[{ "id": "guid", "code": "DE", "name": "Delta Office", "region": "Delta", "city": "Mansoura", "totalSites": 240 }]
```

`EX-55` Office statistics:
```json
{ "officeId": "guid", "officeCode": "DE", "totalSites": 240, "activeSites": 231, "totalEngineers": 28, "scheduledVisits": 46, "lowStockMaterials": 12 }
```

`EX-56` Settings group:
```json
[{ "key": "SLA:P1:ResponseMinutes", "value": "60", "group": "SLA", "dataType": "int", "isEncrypted": false, "updatedBy": "admin@towerops.com" }]
```

`EX-57` Permissions list:
```json
["sites.view", "sites.edit", "visits.view", "visits.approve", "workorders.view"]
```

`EX-58` Delete role response:
- HTTP `204 No Content`.

`EX-59` Suggested order:
```json
[{ "order": 1, "siteCode": "3564DE", "visitType": "BM", "priority": "P1", "distanceFromPreviousKm": 0, "estimatedTravelMinutes": 0 }]
```

`EX-60` Unassigned sites:
```json
[{ "siteId": "guid", "siteCode": "3564DE", "name": "DE-GF-3564" }]
```

`EX-61` Checklist template:
```json
{ "id": "guid", "visitType": "BM", "version": "v1.0", "isActive": true, "items": [{ "id": "guid", "category": "Power", "itemName": "Rectifier Visual Check", "isMandatory": true }] }
```

`EX-62` Checklist history:
```json
[{ "id": "guid", "visitType": "BM", "version": "v1.0", "isActive": false }, { "id": "guid", "visitType": "BM", "version": "v1.1", "isActive": true }]
```

`EX-63` Checklist create response:
```json
"7f568c9e-60a7-4bd1-9b2a-fb8de6a579f0"
```

`EX-64` Asset list:
```json
[{ "id": "guid", "assetCode": "AST-3564DE-REC-001", "siteCode": "3564DE", "type": "Rectifier", "status": "Active" }]
```

`EX-65` Sync status:
```json
{ "deviceId": "android-01", "total": 40, "pending": 3, "processed": 35, "conflicts": 1, "failed": 1, "items": [] }
```

`EX-66` Sync conflicts:
```json
[{ "id": "guid", "syncQueueId": "guid", "conflictType": "VisitAlreadySubmitted", "resolution": "ServerWins", "resolvedAtUtc": "2026-02-25T11:00:00Z" }]
```

---

## 4) Sprint Execution Plan (Frontend + API Integration)

## Sprint 1: Foundations and API Client Core
Goals:
- Stabilize frontend architecture and API integration contracts.
- Implement auth bootstrap and global error handling.

Deliverables:
- React app shell (routing, layouts, guarded routes).
- API client with token handling, error normalization, correlation ID capture.
- Shared TypeScript models for auth, error, pagination.

Required APIs:
- `POST /api/auth/login`
- `POST /api/auth/change-password`
- `GET /api/roles/permissions` (optional admin bootstrap)

UI components:
- Login page
- App shell + nav + session guard
- Global toaster/error banner

Dependencies:
- JWT secret configured in backend env
- CORS for frontend origin

## Sprint 2: Master Data and Admin Workspace
Goals:
- Deliver admin management screens for offices, users, roles, settings.

Deliverables:
- Office CRUD screens
- User CRUD and role change screens
- Roles and permissions matrix
- Settings grouped editor and connection-test actions

Required APIs:
- `/api/offices/*`
- `/api/users/*`
- `/api/roles/*`
- `/api/settings/*`

UI components:
- Data grid with filters
- Edit forms with schema validation
- Permission chips and role matrix

Dependencies:
- Authorization policy mapping in frontend route guards.

## Sprint 3: Site and Material Operations
Goals:
- Enable office/supervisor workflows for site master and inventory.

Deliverables:
- Site detail and office-site list pages
- Site assignment/status/ownership actions
- Site import screens with error grids
- Materials stock, low-stock alerts, stock mutation actions

Required APIs:
- `/api/sites/*`
- `/api/materials/*`
- `/api/assets/*` (read-only in this sprint)

UI components:
- Import wizard + result panel
- Site cards/map points (optional map provider)
- Material stock transactions modal

Dependencies:
- Excel template validation behavior aligned with backend.

## Sprint 4: Visit Lifecycle Workspace
Goals:
- Deliver full field and review lifecycle UX.

Deliverables:
- Engineer visit queue (paginated)
- Visit detail with checklist/readings/photos/issues tabs
- Start/checkin/checkout/complete/submit flows
- Review (approve/reject/request-correction)
- Visit signature capture and display

Required APIs:
- `/api/visits/*`
- `/api/checklisttemplates/*` (read/import/activate for admin)

UI components:
- Evidence checklist progress indicator
- GPS checkin status badge
- Photo upload and evidence timeline

## Sprint 5: Work Orders, Escalations, and Daily Planning
Goals:
- Close operations control loop from issue to execution to closure.

Deliverables:
- Work order board (status lanes)
- Work order lifecycle actions including customer acceptance path
- Escalation lifecycle panel
- Daily plan builder + route suggestion view

Required APIs:
- `/api/workorders/*`
- `/api/escalations/*`
- `/api/daily-plans/*`

UI components:
- Kanban board
- Timeline and SLA badges
- Planner with assignment and suggested order

## Sprint 6: Reporting and Analytics Command Center
Goals:
- Deliver executive dashboard and report center.

Deliverables:
- KPI dashboard page
- Analytics pages for trends/issues/engineer/site/office/material
- Report exports center (scorecard/checklist/bdt/data-collection/visit-report)

Required APIs:
- `/api/kpi/operations`
- `/api/analytics/*`
- `/api/reports/*`

UI components:
- Chart library wrappers (line, bar, donut, gauge)
- Filter panel (date range, office, SLA class)
- Export action bar with download states

## Sprint 7: Client Portal and Offline/Sync Monitoring
Goals:
- Ship external client visibility and reliability dashboards.

Deliverables:
- Client portal dashboard/sites/workorders/sla/visits
- Portal evidence viewer
- Sync health monitor and conflict list for operations

Required APIs:
- `/api/portal/*`
- `/api/sync/*`

UI components:
- Restricted portal shell
- Anonymized engineer display
- Sync queue/conflict tracker

## Sprint 8: Hardening, Accessibility, and UAT
Goals:
- Production readiness and UX quality closure.

Deliverables:
- Accessibility pass (WCAG AA baseline)
- Performance optimization (code splitting, caching, virtualization)
- E2E smoke tests by role
- UAT signoff pack and release notes

Required APIs:
- All production APIs

UI components:
- Audit/error trace panel for support
- Loading skeletons and optimistic update guards

---

## 5) Production-Grade Git Strategy

## 5.1 Branching Model
- `main`: production branch, always deployable, tagged releases only.
- `develop`: integration branch for next release.
- `feature/*`: one feature or vertical slice per branch.
- `release/*`: stabilization branch for release candidate hardening.
- `hotfix/*`: urgent production fix branched from `main`.

### When each branch is used
- `feature/*` from `develop` when implementing sprint work.
- Merge `feature/*` to `develop` only through PR + review.
- Cut `release/x.y.z` from `develop` when scope is frozen.
- Merge `release/*` to `main` (tag) and back to `develop`.
- `hotfix/*` from `main` for production incidents; merge to both `main` and `develop`.

## 5.2 Commit Rules
- Enforce Conventional Commits:
  - `feat:`
  - `fix:`
  - `refactor:`
  - `test:`
  - `docs:`
  - `chore:`
  - `perf:`
  - `ci:`
- Keep commits atomic: one intention per commit.
- Message pattern:
  - `<type>(optional-scope): short imperative summary`
  - body explains why, not only what.

Good examples:
- `feat(reporting): add KPI dashboard widgets and filter state`
- `fix(visits): prevent duplicate photo upload submissions`
- `refactor(api-client): normalize paginated responses`

## 5.3 Workflow Commands

### Start a feature
```bash
git checkout develop
git pull origin develop
git checkout -b feature/sprint6-kpi-dashboard
```

### Commit work
```bash
git add .
git commit -m "feat(kpi): implement operations dashboard filters and cards"
```

### Push and open PR
```bash
git push -u origin feature/sprint6-kpi-dashboard
gh pr create --base develop --head feature/sprint6-kpi-dashboard --title "feat(kpi): dashboard page and analytics integration" --body-file .github/PULL_REQUEST_TEMPLATE.md
```

### Finish feature (after approvals)
```bash
git checkout develop
git pull origin develop
git merge --no-ff feature/sprint6-kpi-dashboard
git push origin develop
git branch -d feature/sprint6-kpi-dashboard
```

### Cut release
```bash
git checkout develop
git pull origin develop
git checkout -b release/v1.2.0
git push -u origin release/v1.2.0
```

### Release to production
```bash
git checkout main
git pull origin main
git merge --no-ff release/v1.2.0
git tag -a v1.2.0 -m "TowerOps v1.2.0"
git push origin main --tags

git checkout develop
git merge --no-ff release/v1.2.0
git push origin develop
```

### Hotfix
```bash
git checkout main
git pull origin main
git checkout -b hotfix/v1.2.1-auth-timeout
# implement fix
git commit -m "fix(auth): handle token refresh timeout on login retry"
git push -u origin hotfix/v1.2.1-auth-timeout
```

After PR:
```bash
git checkout main
git merge --no-ff hotfix/v1.2.1-auth-timeout
git tag -a v1.2.1 -m "TowerOps v1.2.1 hotfix"
git push origin main --tags

git checkout develop
git merge --no-ff hotfix/v1.2.1-auth-timeout
git push origin develop
```

## 5.4 Sprint-Based Git Workflow

Feature-to-branch mapping:
- One story/feature per branch.
- Prefix with sprint and scope:
  - `feature/sprint6-kpi-dashboard`
  - `feature/sprint6-analytics-issue-report`
  - `feature/sprint7-portal-sites-grid`

PR structure:
- Maximum ~500 LOC net change when possible.
- Vertical slices preferred (API integration + UI + tests).
- Required sections in PR description:
  - Problem statement
  - Scope
  - API contracts changed/used
  - Screenshots/video
  - Test evidence
  - Rollback plan

Merge policy:
- Squash merge for feature branches.
- No direct pushes to `develop` or `main`.
- Release/hotfix branches use merge commit for traceability.

Code review policy:
- Minimum 2 approvals for `main`, 1 for `develop`.
- At least one reviewer from backend for contract-impacting PRs.
- Block merge if:
  - failing CI
  - contract drift
  - missing tests for changed behavior
  - unresolved security comments

## 5.5 GitHub Best Practices

Branch protection:
- Protect `main` and `develop`.
- Require PR before merge.
- Require up-to-date branch before merge.
- Require status checks:
  - build
  - tests
  - lint
  - docs drift check
- Require signed commits (if org policy allows).

PR template fields (required):
- Summary
- Linked issue
- Scope checklist
- API/contract impact
- Testing evidence
- Risk and rollback

Commit linting:
- Add commitlint in CI for Conventional Commits.
- Reject non-compliant commit messages.

Versioning:
- Semantic versioning `vMAJOR.MINOR.PATCH`
  - MAJOR: breaking API/contract changes
  - MINOR: backward-compatible feature additions
  - PATCH: backward-compatible fixes

Tagging format:
- `v1.0.0`
- `v1.0.1`
- `v1.1.0`

---

## 6) Recommended Immediate Next Actions
1. Approve this contract document as source of truth for frontend build.
2. Create frontend issue board from Sprint 1-8 sections.
3. Implement API adapter (`ApiSuccess<T>`) in frontend before feature pages.
4. Enforce branch protection + commit lint before the first sprint PR.
5. Add missing P0 reports to backend backlog before go-live commitment.
