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

## Access Model
- All business controllers are protected with `[Authorize]` and selected actions enforce role policies.
- `AuthController` login endpoint is `[AllowAnonymous]`.

## Endpoint Catalog by Controller

### `AuthController` (`/api/auth`)
- `POST /login` (AllowAnonymous)
  - Credentials currently validated by `email + phoneNumber` against active user profile.
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
- `POST /{visitId}/photos`

### `WorkOrdersController` (`/api/workorders`)
- `POST /` (**CanManageWorkOrders**)
- `GET /{workOrderId}` (**CanViewWorkOrders**)
- `POST /{workOrderId}/assign` (**CanManageWorkOrders**)

### `EscalationsController` (`/api/escalations`)
- `POST /` (**CanManageEscalations**)
- `GET /{escalationId}` (**CanViewEscalations**)

### `KpiController` (`/api/kpi`)
- `GET /operations` (**CanViewKpis**)
  - Filters: `fromDateUtc`, `toDateUtc`, `officeCode`, `slaClass`

### `SitesController` (`/api/sites`)
- `GET /{siteId}`
- `GET /office/{officeId}`
- `GET /maintenance`

### `MaterialsController` (`/api/materials`)
- `GET /low-stock/{officeId}`

### `ReportsController` (`/api/reports`)
- `GET /visits/{visitId}`

### `UsersController` (`/api/users`)
- `POST /`
- `GET /{userId}`
- `PUT /{userId}`
- `DELETE /{userId}`
- `PATCH /{userId}/role`
- `PATCH /{userId}/activate`
- `PATCH /{userId}/deactivate`
- `GET /office/{officeId}`
- `GET /role/{role}`
- `GET /{userId}/performance`

### `OfficesController` (`/api/offices`)
- `POST /`
- `GET /{officeId}`
- `GET /`
- `GET /region/{region}`
- `GET /{officeId}/statistics`
- `PUT /{officeId}`
- `PATCH /{officeId}/contact`
- `DELETE /{officeId}`

### `AnalyticsController` (`/api/analytics`)
- `GET /engineer-performance/{engineerId}`
- `GET /site-maintenance/{siteId}`
- `GET /office-statistics/{officeId}`
- `GET /material-usage/{materialId}`
- `GET /visit-completion-trends`
- `GET /issue-analytics`

## Run locally
```bash
dotnet run --project src/TelecomPm.Api
```
