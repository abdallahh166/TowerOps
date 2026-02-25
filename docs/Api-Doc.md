# TowerOps API Documentation

## Purpose
This document describes the current ASP.NET Core API surface in `src/TelecomPm.Api`, including runtime behavior, authorization policies, and endpoint contracts at controller level for the **TowerOps** product by **Seven Pictures**.

Branding note:
- Product brand: `TowerOps`
- Internal code namespaces/project names: `TelecomPM` / `TelecomPm` (kept for compatibility)

## Runtime Architecture
- Framework: ASP.NET Core Web API (`net8.0`)
- API style: controller-based REST endpoints
- Auth: JWT bearer token
- Authorization: policy + permission claims
- Validation: FluentValidation through MediatR pipeline and model-state filter
- Error handling: centralized `ExceptionHandlingMiddleware` with localized messages
- Logging: Serilog + request logging middleware
- Correlation: `X-Correlation-ID` request/response propagation with scoped logging
- Localization: `en-US`, `ar-EG` via query string, `Accept-Language`, or cookie
- Abuse controls: global path-scoped rate limiting for `/api/auth/*`, `/api/sync*`, and `*/import*`
- Transport security: HSTS enabled in Production
- Health checks: `GET /health`
- Swagger/OpenAPI: enabled in Development

## Configuration and Environment
Primary config file: `src/TelecomPm.Api/appsettings.json`

Critical settings:
- `ConnectionStrings:DefaultConnection`
- `JwtSettings:Issuer`
- `JwtSettings:Audience`
- `Localization:*`
- `Cors:AllowedOrigins` (required and explicit in Production)
- `RateLimiting:*`
- `Hsts:*`
- `Settings:EncryptionKey`

Environment variables:
- `JWT_SECRET`
  - required in Production (startup guard)
  - Development fallback can come from `appsettings.Development.json`

## Security Model
### Authentication
- `POST /api/auth/login` is anonymous and returns JWT.
- Other protected endpoints require bearer token.

### Authorization policies
Defined in `src/TelecomPm.Api/Authorization/ApiAuthorizationPolicies.cs`:
- `CanManageWorkOrders`
- `CanViewWorkOrders`
- `CanManageVisits`
- `CanViewVisits`
- `CanReviewVisits`
- `CanCreateEscalations`
- `CanManageEscalations`
- `CanViewEscalations`
- `CanViewKpis`
- `CanManageUsers`
- `CanViewUsers`
- `CanManageOffices`
- `CanManageSites`
- `CanViewSites`
- `CanViewAnalytics`
- `CanViewReports`
- `CanViewMaterials`
- `CanManageMaterials`
- `CanManageSettings`
- `CanViewPortal`
- `CanManagePortalWorkOrders`

Policies evaluate permission claims (`PermissionConstants.ClaimType`), not hardcoded roles.

## Data Flow (Request Path)
1. HTTP request reaches controller endpoint.
2. AuthN/AuthZ middleware validates token and policy.
3. Controller maps contract DTOs to commands/queries.
4. MediatR pipeline executes:
   - unhandled exception behavior
   - logging behavior
   - validation behavior
   - performance behavior
   - transaction behavior (commands)
5. Handler executes domain/application logic and persistence via repositories/unit of work.
6. Domain events are dispatched from `ApplicationDbContext.SaveChangesAsync`.
7. Controller returns standardized success/failure envelope through `ApiControllerBase.HandleResult`.

## Controllers and Endpoints

### AuthController (`/api/auth`)
- `POST /login` (`AllowAnonymous`)
- `POST /forgot-password` (`AllowAnonymous`)
- `POST /reset-password` (`AllowAnonymous`)
- `POST /change-password` (`Authorize`)

### VisitsController (`/api/visits`) class policy: `CanViewVisits`
- `GET /{visitId}`
- `GET /engineers/{engineerId}`
- `GET /pending-reviews`
- `GET /scheduled`
- `GET /{visitId}/evidence-status`
- `GET /{visitId}/signature`
- `POST /` (`CanManageVisits`)
- `POST /{visitId}/start` (`CanManageVisits`)
- `POST /{visitId}/checkin` (`CanManageVisits`)
- `POST /{visitId}/checkout` (`CanManageVisits`)
- `POST /{visitId}/complete` (`CanManageVisits`)
- `POST /{visitId}/submit` (`CanManageVisits`)
- `POST /{visitId}/approve` (`CanReviewVisits`)
- `POST /{visitId}/reject` (`CanReviewVisits`)
- `POST /{visitId}/request-correction` (`CanReviewVisits`)
- `POST /{visitId}/checklist-items` (`CanManageVisits`)
- `PATCH /{visitId}/checklist-items/{checklistItemId}` (`CanManageVisits`)
- `POST /{visitId}/issues` (`CanManageVisits`)
- `POST /{visitId}/issues/{issueId}/resolve` (`CanManageVisits`)
- `POST /{visitId}/readings` (`CanManageVisits`)
- `PATCH /{visitId}/readings/{readingId}` (`CanManageVisits`)
- `POST /{visitId}/photos` (`CanManageVisits`)
- `DELETE /{visitId}/photos/{photoId}` (`CanManageVisits`)
- `POST /{visitId}/cancel` (`CanManageVisits`)
- `POST /{visitId}/reschedule` (`CanManageVisits`)
- `POST /{visitId}/signature` (`CanManageVisits`)
- `POST /{visitId}/import/panorama` (`CanManageVisits`)
- `POST /{visitId}/import/alarms` (`CanManageVisits`)
- `POST /{visitId}/import/unused-assets` (`CanManageVisits`)

### WorkOrdersController (`/api/workorders`) class policy: `Authorize`
- `POST /` (`CanManageWorkOrders`)
- `GET /{workOrderId}` (`CanViewWorkOrders`)
- `POST /{workOrderId}/assign` (`CanManageWorkOrders`)
- `PATCH /{id}/start` (`CanManageWorkOrders`)
- `PATCH /{id}/complete` (`CanManageWorkOrders`)
- `PATCH /{id}/close` (`CanManageWorkOrders`)
- `PATCH /{id}/cancel` (`CanManageWorkOrders`)
- `PATCH /{id}/submit-for-acceptance` (`CanManageWorkOrders`)
- `PATCH /{id}/customer-accept` (`CanManageWorkOrders`)
- `PATCH /{id}/customer-reject` (`CanManageWorkOrders`)
- `POST /{id}/signature` (`CanManageWorkOrders`)
- `GET /{id}/signature` (`CanViewWorkOrders`)

### EscalationsController (`/api/escalations`) class policy: `Authorize`
- `POST /` (`CanCreateEscalations`)
- `GET /{escalationId}` (`CanViewEscalations`)
- `PATCH /{id}/review` (`CanManageEscalations`)
- `PATCH /{id}/approve` (`CanManageEscalations`)
- `PATCH /{id}/reject` (`CanManageEscalations`)
- `PATCH /{id}/close` (`CanManageEscalations`)

### SitesController (`/api/sites`) class policy: `CanViewSites`
- `GET /{siteId}`
- `GET /{siteCode}/location`
- `GET /office/{officeId}`
- `GET /maintenance`
- `POST /` (`CanManageSites`)
- `PUT /{siteId}` (`CanManageSites`)
- `PATCH /{siteId}/status` (`CanManageSites`)
- `POST /{siteId}/assign` (`CanManageSites`)
- `POST /{siteId}/unassign` (`CanManageSites`)
- `PUT /{siteCode}/ownership` (`CanManageSites`)
- `POST /import` (`CanManageSites`)
- `POST /import/site-assets` (`CanManageSites`)
- `POST /import/power-data` (`CanManageSites`)
- `POST /import/radio-data` (`CanManageSites`)
- `POST /import/tx-data` (`CanManageSites`)
- `POST /import/sharing-data` (`CanManageSites`)
- `POST /import/rf-status` (`CanManageSites`)
- `POST /import/battery-discharge-tests` (`CanManageSites`)
- `POST /import/delta-sites` (`CanManageSites`)

### MaterialsController (`/api/materials`) class policy: `CanViewMaterials`
- `GET /{id}`
- `GET /`
- `GET /low-stock/{officeId}`
- `POST /` (`CanManageMaterials`)
- `PUT /{id}` (`CanManageMaterials`)
- `DELETE /{id}` (`CanManageMaterials`)
- `POST /{id}/stock/add` (`CanManageMaterials`)
- `POST /{id}/stock/reserve` (`CanManageMaterials`)
- `POST /{id}/stock/consume` (`CanManageMaterials`)

### ReportsController (`/api/reports`) class policy: `CanViewReports`
- `GET /visits/{visitId}`
- `GET /scorecard`
- `GET /checklist`
- `GET /bdt`
- `GET /data-collection`

### AnalyticsController (`/api/analytics`) class policy: `CanViewAnalytics`
- `GET /engineer-performance/{engineerId}`
- `GET /site-maintenance/{siteId}`
- `GET /office-statistics/{officeId}`
- `GET /material-usage/{materialId}`
- `GET /visit-completion-trends`
- `GET /issue-analytics`

### KpiController (`/api/kpi`) class policy: `Authorize`
- `GET /operations` (`CanViewKpis`)

### UsersController (`/api/users`) class policy: `CanViewUsers`
- `GET /{userId}`
- `GET /office/{officeId}`
- `GET /role/{role}`
- `GET /{userId}/performance`
- `POST /` (`CanManageUsers`)
- `PUT /{userId}` (`CanManageUsers`)
- `DELETE /{userId}` (`CanManageUsers`)
- `PATCH /{userId}/role` (`CanManageUsers`)
- `PATCH /{userId}/activate` (`CanManageUsers`)
- `PATCH /{userId}/deactivate` (`CanManageUsers`)

### OfficesController (`/api/offices`) class policy: `CanManageOffices`
- `POST /`
- `GET /{officeId}`
- `GET /` (supports `onlyActive`, `pageNumber`, `pageSize`; `pageSize` capped at 200)
- `GET /region/{region}`
- `GET /{officeId}/statistics`
- `PUT /{officeId}`
- `PATCH /{officeId}/contact`
- `DELETE /{officeId}`

### ChecklistTemplatesController (`/api/checklisttemplates`) class policy: `Authorize`
- `GET /`
- `GET /{id}`
- `GET /history`
- `POST /` (`CanManageWorkOrders`)
- `POST /{id}/activate` (`CanManageWorkOrders`)
- `POST /import` (`CanManageWorkOrders`)

### AssetsController (`/api/assets`) class policy: `Authorize`
- `GET /site/{siteCode}` (`CanViewSites`)
- `GET /{assetCode}` (`CanViewSites`)
- `GET /{assetCode}/history` (`CanViewSites`)
- `POST /` (`CanManageSites`)
- `PUT /{assetCode}/service` (`CanManageSites`)
- `PUT /{assetCode}/fault` (`CanManageSites`)
- `PUT /{assetCode}/replace` (`CanManageSites`)
- `GET /expiring-warranties` (`CanViewSites`)
- `GET /faulty` (`CanViewSites`)

### ClientPortalController (`/api/portal`) class policy: `CanViewPortal`
- `GET /dashboard`
- `GET /sites`
- `GET /sites/{siteCode}`
- `GET /workorders`
- `GET /sla-report`
- `GET /visits/{siteCode}`
- `GET /visits/{visitId}/evidence`
- `PATCH /workorders/{id}/accept` (`CanManagePortalWorkOrders`)
- `PATCH /workorders/{id}/reject` (`CanManagePortalWorkOrders`)

### DailyPlansController (`/api/daily-plans`) class policy: `CanManageSites`
- `POST /`
- `GET /{officeId}/{date}`
- `POST /{planId}/assign`
- `DELETE /{planId}/assign`
- `GET /{planId}/suggest/{engineerId}`
- `GET /{officeId}/{date}/unassigned`
- `POST /{planId}/publish`

### SyncController (`/api/sync`) class policy: `CanManageVisits`
- `POST /`
- `GET /status/{deviceId}`
- `GET /conflicts/{engineerId}`

### SettingsController (`/api/settings`) class policy: `CanManageSettings`
- `GET /` (supports `pageNumber`, `pageSize`; `pageSize` capped at 200)
- `GET /{group}`
- `PUT /`
- `POST /test/{service}`

### RolesController (`/api/roles`) class policy: `CanManageSettings`
- `GET /` (supports `pageNumber`, `pageSize`; `pageSize` capped at 200)
- `GET /permissions`
- `GET /{id}`
- `POST /`
- `PUT /{id}`
- `DELETE /{id}`

## Error Handling and Localization
- Exceptions are normalized by `ExceptionHandlingMiddleware`.
- Domain/Application exceptions support localized message keys.
- Validation errors return structured `Errors` dictionary and localized messages.

Unified error contract for failed requests:
```json
{
  "code": "request.failed",
  "message": "Human-readable localized message",
  "correlationId": "X-Correlation-ID value",
  "errors": {
    "fieldName": ["validation message"]
  },
  "meta": {
    "Rule": "BusinessRuleName"
  }
}
```
Notes:
- `errors` and `meta` are optional and appear only when applicable.
- Stable error codes include: `internal.error`, `request.failed`, `request.validation_failed`,
  `resource.not_found`, `auth.unauthorized`, `auth.forbidden`, `request.conflict`,
  `business.rule_violation`.

## Operational Notes
- All business timestamps are UTC in domain/application logic.
- Request/response behaviors are mediated through commands/queries; controllers do not contain core business logic.
- Operational metrics are emitted from meter `TelecomPM.Operations` for import/sync/notification flows.
- Keep this file aligned with controller route/attribute changes.
