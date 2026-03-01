# Backend Auto-Discovery Report

Date: 2026-02-28  
System: TowerOps backend (`src/TowerOps.Api`, `src/TowerOps.Application`, `src/TowerOps.Domain`, `src/TowerOps.Infrastructure`)

## Purpose
This report captures backend behavior discovered directly from code and separates:
- `Inferred from code`
- `Assumed defaults`
- `Business clarification required`

---

## 1) Inferred from code

## 1.1 Auth model
- Auth is JWT Bearer.
- Login uses `email + password` and verifies password hash via ASP.NET Core `IPasswordHasher<User>`.
- JWT includes claims:
  - `nameidentifier` (user id)
  - `email`
  - `role`
  - `OfficeId`
  - `permission` (one claim per permission)
- Token permissions are resolved from:
  1. DB role (`ApplicationRole.Permissions`) if available
  2. fallback defaults (`RolePermissionDefaults`)
- Forgot/reset password flow exists:
  - OTP is 6 digits
  - OTP is hashed in storage
  - expiry is 15 minutes
  - forgot endpoint is non-disclosing (does not reveal whether email exists)
- Password complexity enforced in validators:
  - min length 8
  - at least one uppercase
  - at least one digit

Reference files:
- `src/TowerOps.Api/Program.cs`
- `src/TowerOps.Api/Controllers/AuthController.cs`
- `src/TowerOps.Api/Services/JwtTokenService.cs`
- `src/TowerOps.Application/Commands/Auth/*`
- `src/TowerOps.Domain/Entities/Users/User.cs`

## 1.2 Roles and permissions
- Core enum roles:
  - `Admin`, `Manager`, `Supervisor`, `PMEngineer`, `Technician`
- Dynamic role aggregate exists: `ApplicationRole` (`Name`, `DisplayName`, `IsSystem`, `IsActive`, `Permissions`)
- Seeded non-enum roles also exist:
  - `Viewer`
  - `ClientPortal`
- Permissions are string-based constants and policy checks are claim-based (`claim type = permission`).

Reference files:
- `src/TowerOps.Domain/Enums/UserEnums.cs`
- `src/TowerOps.Domain/Entities/ApplicationRoles/ApplicationRole.cs`
- `src/TowerOps.Application/Security/PermissionConstants.cs`
- `src/TowerOps.Application/Security/RolePermissionDefaults.cs`
- `src/TowerOps.Api/Authorization/ApiAuthorizationPolicies.cs`
- `src/TowerOps.Infrastructure/Persistence/SeedData/DatabaseSeeder.cs`

## 1.3 API contract model
- Success responses are not globally wrapped; endpoints usually return DTOs, lists, paginated lists, or files directly.
- Application layer uses `Result` / `Result<T>`, mapped in API base controller.
- Error responses are standardized through middleware/factory.

Error payload shape:
```json
{
  "code": "request.validation_failed",
  "message": "Validation failed",
  "correlationId": "0HNJK...",
  "errors": {
    "field": ["message"]
  },
  "meta": {
    "Rule": "..."
  }
}
```

Error codes discovered:
- `internal.error`
- `request.failed`
- `request.validation_failed`
- `resource.not_found`
- `auth.unauthorized`
- `auth.forbidden`
- `request.conflict`
- `business.rule_violation`

Reference files:
- `src/TowerOps.Api/Controllers/ApiControllerBase.cs`
- `src/TowerOps.Api/Errors/ApiErrorResponse.cs`
- `src/TowerOps.Api/Errors/ApiErrorFactory.cs`
- `src/TowerOps.Api/Errors/ApiErrorCodes.cs`
- `src/TowerOps.Api/Middleware/ExceptionHandlingMiddleware.cs`

## 1.4 Pagination/filtering model
- Common paginated response type exists: `PaginatedList<T>`
  - `Items`, `PageNumber`, `TotalPages`, `TotalCount`, `HasPreviousPage`, `HasNextPage`
- Pagination inputs vary by endpoint; common pattern is:
  - `pageNumber`
  - `pageSize` (often clamped to max 200 in handlers)
- Filtering is endpoint-specific (date, status, office, SLA class, etc.).

Reference files:
- `src/TowerOps.Application/Common/PaginatedList.cs`
- `src/TowerOps.Api/Contracts/*/*QueryParameters.cs`
- `src/TowerOps.Application/Queries/*`

## 1.5 File handling rules
- Form uploads (`IFormFile`) used for:
  - visit photos
  - site/visit/checklist imports (Excel)
- Visit photo endpoint has explicit request size limit: `25 MB`.
- File stream processing pattern:
  - photos streamed to storage via `IFileStorageService`
  - imports read as `byte[]`, then validated
- Import guardrails include:
  - file presence
  - max file size (default 10 MB, configurable)
  - Excel signature check
  - max row count (default 5000, configurable)
  - optional skip-invalid behavior
- Signature payload validation enforces:
  - valid base64
  - PNG format
  - max size 150 KB

Reference files:
- `src/TowerOps.Api/Controllers/VisitsController.cs`
- `src/TowerOps.Api/Controllers/SitesController.cs`
- `src/TowerOps.Application/Commands/Imports/ImportGuardrails.cs`
- `src/TowerOps.Domain/ValueObjects/Signature.cs`

## 1.6 Security middleware and hardening
- Middleware pipeline includes:
  - correlation id middleware
  - request logging middleware
  - exception handling middleware
- Security controls discovered:
  - JWT authentication
  - policy authorization
  - CORS allow-list
  - global rate limiting (Auth / Sync / Import buckets)
  - HTTPS redirection
  - HSTS in production
  - health check endpoint `/health`
- Production guard:
  - `JWT_SECRET` must be present in production environment.

Reference files:
- `src/TowerOps.Api/Program.cs`
- `src/TowerOps.Api/Security/ApiSecurityHardening.cs`
- `src/TowerOps.Api/Middleware/*.cs`

## 1.7 API inventory (controller-level)
| Controller | Base route | Endpoint count |
|---|---|---:|
| AuthController | `api/auth` | 4 |
| UsersController | `api/users` | 10 |
| RolesController | `api/roles` | 6 |
| SettingsController | `api/settings` | 4 |
| SitesController | `api/sites` | 19 |
| VisitsController | `api/visits` | 29 |
| WorkOrdersController | `api/workorders` | 12 |
| EscalationsController | `api/escalations` | 6 |
| MaterialsController | `api/materials` | 9 |
| OfficesController | `api/offices` | 8 |
| DailyPlansController | `api/daily-plans` | 7 |
| SyncController | `api/sync` | 3 |
| AssetsController | `api/assets` | 9 |
| AnalyticsController | `api/analytics` | 6 |
| KpiController | `api/kpi` | 1 |
| ReportsController | `api/reports` | 5 |
| ClientPortalController | `api/portal` | 9 |
| ChecklistTemplatesController | `api/checklisttemplates` | 6 |

---

## 2) Assumed defaults
- JSON naming convention is assumed frontend-default style unless overridden by serializer settings elsewhere.
- API versioning is assumed single-version (no explicit route versioning attributes discovered).
- UTC is assumed for business timestamps based on field naming and handling patterns.
- Session revocation strategy is assumed stateless JWT unless implemented outside this codebase.

---

## 3) Business Confirmation Status

Source of truth:
- `docs/Business-Confirmation-Checklist.md`

### 3.1 Confirmed decisions (approved policy)
1. BC-01 Session policy (`#47`)
   - Access token TTL 15 minutes.
   - Refresh token TTL 7 days, rotating per use.
   - Server-side revocation list required.
   - Password change must revoke all user refresh tokens.
2. BC-02 Account protection (`#48`)
   - 5 failed attempts -> 15-minute lockout.
   - Third lockout in 24h -> manual admin unlock.
   - MFA mandatory for Admin + Manager, optional for Engineer/Viewer.
3. BC-03 Portal workorder permissions (`#49`)
   - Split `portal.view_workorders` (read) and `portal.manage_workorders` (mutate).
   - Default new portal users to view-only.
4. BC-04 Error contract standard (`#50`)
   - Reject deferred-v2 contract approach.
   - Enforce one structured error envelope across endpoints (ProblemDetails-style).
5. BC-07 Pagination standard (`#53`)
   - Standard request: `page`, `pageSize`, `sortBy`, `sortDir`.
   - Standard response: `{ data, pagination }`.
   - `sortBy` must be server allowlist-validated.

### 3.2 Implementation status for approved decisions
1. BC-05 Data retention and privacy (`#51`)
   - Approved, implementation pending in tracking issue `#62`.
2. BC-06 Upload security policy (`#52`)
   - Approved and implemented in PR `#66`, linked delivery issue `#64` closed.
3. BC-08 SLA governance finalization (`#54`)
   - Approved and implemented in PR `#65`, linked delivery issue `#63` closed.

---

## 4) Notes
- This report is discovery-based and implementation-grounded.
- It should be updated whenever auth, policies, middleware, or API conventions change.
