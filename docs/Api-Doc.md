# TelecomPM API Layer

## Overview
The API layer exposes domain/application capabilities over RESTful ASP.NET Core Web API (`net8.0`). It follows Clean Architecture and keeps business rules in Application + Domain.

## Runtime Setup
- JWT authentication (`JwtSettings`)
- Policy-based authorization
- Serilog request/error logging
- Swagger/OpenAPI
- Health checks (`/health`)
- CORS from `Cors:AllowedOrigins`

## Authorization Policies
- `CanManageWorkOrders`: Admin, Manager, Supervisor
- `CanViewWorkOrders`: Admin, Manager, Supervisor, PMEngineer
- `CanReviewVisits`: Admin, Manager, Supervisor
- `CanManageEscalations`: Admin, Manager, Supervisor
- `CanViewEscalations`: Admin, Manager, Supervisor, PMEngineer
- `CanViewKpis`: Admin, Manager, Supervisor
- `CanManageUsers`: Admin, Manager
- `CanManageOffices`: Admin, Manager
- `CanManageSites`: Admin, Manager, Supervisor
- `CanViewSites`: Admin, Manager, Supervisor, PMEngineer
- `CanViewAnalytics`: Admin, Manager, Supervisor, PMEngineer
- `CanViewReports`: Admin, Manager, Supervisor, PMEngineer
- `CanViewMaterials`: Admin, Manager, Supervisor, PMEngineer
- `CanManageMaterials`: Admin, Manager, Supervisor

## Access Model
- All business controllers are protected with `[Authorize]` and selected actions enforce role policies.
- `AuthController` login endpoint is `[AllowAnonymous]`.

## Endpoint Catalog by Controller

### `AuthController` (`/api/auth`)
- `POST /login` (AllowAnonymous)
  - Credentials validated by `email + password` for active users.
  - Returns JWT access token with `NameIdentifier`, `Email`, `Role`, `OfficeId` claims.

### `VisitsController` (`/api/visits`)
- `GET /{visitId}`
- `GET /engineers/{engineerId}`
- `GET /pending-reviews`
- `GET /scheduled`
- `POST /`
- `GET /{visitId}/evidence-status`
- `POST /{visitId}/start`
- `POST /{visitId}/complete`
- `POST /{visitId}/submit`
- `POST /{visitId}/approve` (**CanReviewVisits**)
- `POST /{visitId}/reject` (**CanReviewVisits**)
- `POST /{visitId}/request-correction` (**CanReviewVisits**)
- `POST /{visitId}/checklist-items`
- `PATCH /{visitId}/checklist-items/{checklistItemId}`
- `POST /{visitId}/issues`
- `POST /{visitId}/issues/{issueId}/resolve`
- `POST /{visitId}/readings`
- `PATCH /{visitId}/readings/{readingId}`
- `POST /{visitId}/photos`
- `POST /{visitId}/import/panorama`
- `POST /{visitId}/import/alarms`
- `DELETE /{visitId}/photos/{photoId}`
- `POST /{visitId}/cancel`
- `POST /{visitId}/reschedule`

### `WorkOrdersController` (`/api/workorders`)
- `POST /` (**CanManageWorkOrders**)
- `GET /{workOrderId}` (**CanViewWorkOrders**)
- `POST /{workOrderId}/assign` (**CanManageWorkOrders**)
- `PATCH /{id}/start` (**CanManageWorkOrders**)
- `PATCH /{id}/complete` (**CanManageWorkOrders**)
- `PATCH /{id}/close` (**CanManageWorkOrders**)
- `PATCH /{id}/cancel` (**CanManageWorkOrders**)
- `PATCH /{id}/submit-for-acceptance` (**CanManageWorkOrders**)
- `PATCH /{id}/customer-accept` (**CanManageWorkOrders**)
- `PATCH /{id}/customer-reject` (**CanManageWorkOrders**)

### `EscalationsController` (`/api/escalations`)
- `POST /` (**CanManageEscalations**)
- `GET /{escalationId}` (**CanManageEscalations**)
- `PATCH /{id}/review` (**CanManageEscalations**)
- `PATCH /{id}/approve` (**CanManageEscalations**)
- `PATCH /{id}/reject` (**CanManageEscalations**)
- `PATCH /{id}/close` (**CanManageEscalations**)

### `KpiController` (`/api/kpi`)
- `GET /operations` (**CanViewKpis**)
  - Filters: `fromDateUtc`, `toDateUtc`, `officeCode`, `slaClass`

### `SitesController` (`/api/sites`)
- `POST /` (**CanManageSites**)
- `PUT /{siteId}` (**CanManageSites**)
- `PATCH /{siteId}/status` (**CanManageSites**)
- `POST /{siteId}/assign` (**CanManageSites**)
- `POST /{siteId}/unassign` (**CanManageSites**)
- `POST /import` (**CanManageSites**) - GH-DE Data Collection import
- `POST /import/site-assets` (**CanManageSites**)
- `POST /import/power-data` (**CanManageSites**)
- `POST /import/radio-data` (**CanManageSites**)
- `POST /import/tx-data` (**CanManageSites**)
- `POST /import/sharing-data` (**CanManageSites**)
- `POST /import/rf-status` (**CanManageSites**)
- `POST /import/battery-discharge-tests` (**CanManageSites**)
- `POST /import/delta-sites` (**CanManageSites**)
- `GET /{siteId}` (**CanViewSites**)
- `GET /office/{officeId}` (**CanViewSites**)
- `GET /maintenance` (**CanViewSites**)

### `MaterialsController` (`/api/materials`)
- `POST /` (**CanManageMaterials**)
- `PUT /{id}` (**CanManageMaterials**)
- `DELETE /{id}` (**CanManageMaterials**)
- `POST /{id}/stock/add` (**CanManageMaterials**)
- `POST /{id}/stock/reserve` (**CanManageMaterials**)
- `POST /{id}/stock/consume` (**CanManageMaterials**)
- `GET /{id}` (**CanViewMaterials**)
- `GET /` (**CanViewMaterials**)
- `GET /low-stock/{officeId}`

### `ChecklistTemplatesController` (`/api/checklisttemplates`)
- `GET /?visitType={visitType}`
- `GET /{id}`
- `GET /history?visitType={visitType}`
- `POST /import` (**CanManageWorkOrders**)
- `POST /` (**CanManageWorkOrders**)
- `POST /{id}/activate` (**CanManageWorkOrders**)

### `ReportsController` (`/api/reports`)
- `GET /visits/{visitId}`
- `GET /scorecard?officeCode={officeCode}&month={month}&year={year}`
- `GET /checklist?visitId={visitId?}&visitType={visitType?}`
- `GET /bdt?fromDateUtc={from?}&toDateUtc={to?}`
- `GET /data-collection?officeCode={officeCode?}`

### `UsersController` (`/api/users`)
- `POST /` (**CanManageUsers**)
- `GET /{userId}`
- `PUT /{userId}` (**CanManageUsers**)
- `DELETE /{userId}` (**CanManageUsers**)
- `PATCH /{userId}/role` (**CanManageUsers**)
- `PATCH /{userId}/activate` (**CanManageUsers**)
- `PATCH /{userId}/deactivate` (**CanManageUsers**)
- `GET /office/{officeId}`
- `GET /role/{role}`
- `GET /{userId}/performance`

### `OfficesController` (`/api/offices`)
- `POST /` (**CanManageOffices**)
- `GET /{officeId}`
- `GET /`
- `GET /region/{region}`
- `GET /{officeId}/statistics`
- `PUT /{officeId}` (**CanManageOffices**)
- `PATCH /{officeId}/contact` (**CanManageOffices**)
- `DELETE /{officeId}` (**CanManageOffices**)

### `AnalyticsController` (`/api/analytics`)
- `GET /engineer-performance/{engineerId}`
- `GET /site-maintenance/{siteId}`
- `GET /office-statistics/{officeId}`
- `GET /material-usage/{materialId}`
- `GET /visit-completion-trends`
- `GET /issue-analytics`
  - All analytics endpoints require **CanViewAnalytics**

## Run locally
```bash
dotnet run --project src/TelecomPm.Api
```
