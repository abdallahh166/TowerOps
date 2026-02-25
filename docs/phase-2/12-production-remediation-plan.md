# Phase 2 Production Remediation Plan (P0/P1/P2)

Date: 2026-02-24  
Status: In Progress  
Owner: Engineering

## 1) Current Readiness Snapshot

Current status: **Partially Ready**.

This plan converts the latest architecture/security/performance findings into an execution backlog with release gates.

## 2) Prioritization Rules

- **P0**: Security/data integrity/runtime failure risks. Must be closed before production.
- **P1**: Performance/scalability/operational resilience. Must be closed before scale rollout.
- **P2**: Maintainability and optimization. Can follow immediately after go-live if mitigations exist.

## 3) PR Slice Plan

## P0 (Release Blockers)

### PR-P0-01: Authorization Boundary Hardening

Scope:
- Split coarse mutation policies into action-level policies where needed.
- Lock `ClientPortalController` mutation endpoints (`PATCH /api/portal/workorders/{id}/accept|reject`) behind dedicated mutation policy (not view policy).
- Align escalation mutation permissions so create/review/approve/close are not over-granted or under-granted.

Primary files:
- `src/TowerOps.Api/Authorization/ApiAuthorizationPolicies.cs`
- `src/TowerOps.Api/Controllers/ClientPortalController.cs`
- `src/TowerOps.Api/Controllers/EscalationsController.cs`
- `tests/TowerOps.Application.Tests/Services/ApiAuthorizationPoliciesTests.cs`

Acceptance criteria:
- [ ] Portal user with only `portal.view_*` claims gets `403` on portal workorder accept/reject.
- [ ] Internal authorized roles can execute required mutations.
- [ ] Unauthorized roles cannot execute out-of-scope mutations.
- [ ] Policy tests cover positive and negative cases for each changed policy.

---

### PR-P0-02: Soft Delete Filter Integrity

Scope:
- Fix global query filter in `ApplicationDbContext` so soft-delete applies to all soft-deletable entities (including non-`Guid` aggregate keys such as roles).
- Add regression tests proving deleted roles/settings (and other affected entities) are excluded from normal reads.

Primary files:
- `src/TowerOps.Infrastructure/Persistence/ApplicationDbContext.cs`
- `tests/TowerOps.Infrastructure.Tests/*` (new/updated EF filter tests)

Acceptance criteria:
- [ ] Soft-deleted `ApplicationRole` records are not returned by normal repository queries.
- [ ] Existing `Guid`-key aggregate behavior remains unchanged.
- [ ] EF query filter tests pass and cover both key types.

---

### PR-P0-03: User Role Change Safety Fix

Scope:
- Fix `ChangeUserRoleCommandHandler` demotion path from `PMEngineer` that currently calls invalid capacity reset (`SetEngineerCapacity(0, ...)`).
- Introduce explicit domain-safe engineer profile clearing behavior (or equivalent) and use it from handler.
- Add command/domain tests for role demotion edge cases.

Primary files:
- `src/TowerOps.Application/Commands/Users/ChangeUserRole/ChangeUserRoleCommandHandler.cs`
- `src/TowerOps.Domain/Entities/Users/User.cs` (if domain method needed)
- `tests/TowerOps.Application.Tests/Commands/Users/*`
- `tests/TowerOps.Domain.Tests/Entities/*User*`

Acceptance criteria:
- [ ] Changing role from `PMEngineer` to non-engineer succeeds without exception.
- [ ] Engineer assignments/specializations are cleared according to business rules.
- [ ] Existing user role tests remain green.

---

### PR-P0-04: Blob Storage Configuration Fail-Safe

Scope:
- Make `BlobStorageService` resolve connection string robustly:
  - `AzureBlobStorage:ConnectionString`
  - fallback `ConnectionStrings:AzureBlobStorage`
  - fallback `ConnectionStrings:AzureStorage`
- Add explicit startup/runtime guard with actionable error when unresolved.
- Add unit tests for configuration resolution paths.

Primary files:
- `src/TowerOps.Infrastructure/Services/BlobStorageService.cs`
- `src/TowerOps.Api/appsettings*.json` (if key normalization required)
- `tests/TowerOps.Infrastructure.Tests/Services/*Blob*`

Acceptance criteria:
- [ ] Service starts successfully when any supported config key is present.
- [ ] Service fails fast with clear message when no connection string is configured.
- [ ] Tests cover all resolution branches.

---

### PR-P0-05: Exception Message Sanitization

Scope:
- Remove direct `ex.Message` leakage from command/query results returned to API clients.
- Standardize user-facing messages and keep full details only in logs.
- Add middleware/application tests to verify sanitized error outputs.

Primary files:
- `src/TowerOps.Application/Commands/**`
- `src/TowerOps.Api/Middleware/ExceptionHandlingMiddleware.cs`
- `tests/TowerOps.Application.Tests/Middleware/*`

Acceptance criteria:
- [ ] No endpoint returns raw framework/driver exception text.
- [ ] Correlation/log context still retains diagnostic details server-side.
- [ ] Localization-compatible error keys/messages are preserved.

## P1 (Pre-Scale Hardening)

### PR-P1-01: Query Efficiency and Pagination Sweep

Scope:
- Replace remaining `GetAllAsNoTrackingAsync()` usage in hot read paths with server-side filtered projections.
- Enforce bounded pagination (page size caps) for list endpoints.
- Add query-level tests proving bounded row reads and server-side filtering.

Primary files:
- `src/TowerOps.Application/Queries/**`
- `src/TowerOps.Infrastructure/Persistence/Repositories/**`
- `tests/TowerOps.Application.Tests/Queries/**`

Acceptance criteria:
- [ ] Hot handlers no longer materialize full tables for filtered/paged requests.
- [ ] All list endpoints enforce max page size.
- [ ] Query tests prove filters/pagination execute at DB level.

---

### PR-P1-02: Import Guardrails and Operational Limits

Scope:
- Enforce `Import:*` settings in import handlers (`MaxRows`, `SkipInvalidRows`, format constraints).
- Add file-size/type validation and consistent per-row error reporting.
- Ensure reconciliation outputs include imported/skipped/errors by entity.

Primary files:
- `src/TowerOps.Application/Commands/*Import*`
- `src/TowerOps.Api/Controllers/*Import*`
- `tests/TowerOps.Application.Tests/Integration/Import*`

Acceptance criteria:
- [ ] Oversized/invalid files are rejected deterministically.
- [ ] Import row limit is enforced from settings.
- [ ] Reconciliation output is generated and traceable.

---

### PR-P1-03: HTTP Security and Abuse Controls

Scope:
- Add rate limiting for sensitive endpoints (`/api/auth/*`, `/api/sync`, bulk import endpoints).
- Enable production HSTS policy in API startup.
- Validate CORS policy strictness for production origins.

Primary files:
- `src/TowerOps.Api/Program.cs`
- `src/TowerOps.Api/appsettings*.json`
- `tests/TowerOps.Application.Tests/*` (new API behavior tests if present)

Acceptance criteria:
- [ ] Rate limit is applied and verified on targeted routes.
- [ ] HSTS enabled in production path.
- [ ] CORS does not allow unintended origins in production configuration.

---

### PR-P1-04: Observability Completion

Scope:
- Ensure security-sensitive flows emit structured logs with correlation ID.
- Add failure counters/latency metrics for imports, sync, notification HTTP clients.
- Define alert thresholds and operational runbook references.

Primary files:
- `src/TowerOps.Api/Middleware/RequestLoggingMiddleware.cs`
- `src/TowerOps.Infrastructure/Services/*`
- `docs/phase-2/*` operational docs

Acceptance criteria:
- [ ] Correlation ID appears in request-to-handler logs.
- [ ] Import/sync/notifications expose measurable metrics.
- [ ] Alert/runbook documentation exists and is linked.

## P2 (Post-Go-Live Stabilization)

### PR-P2-01: Error Contract Unification

Scope:
- Introduce consistent error code contract across handlers and middleware.
- Replace free-text failure strings with standardized codes + localized messages.

Acceptance criteria:
- [ ] API errors follow one schema across modules.
- [ ] Client teams can map errors by stable code.

---

### PR-P2-02: Domain/Docs Drift Automation

Scope:
- Expand doc drift checks beyond controllers to policies, critical commands, and imports/exports.
- Add CI gate for documentation consistency.

Acceptance criteria:
- [x] CI fails when critical runtime surfaces are undocumented.
- [x] `docs/Api-Doc.md` and phase docs stay aligned with code changes.

---

### PR-P2-03: Performance and Capacity Baselines

Scope:
- Add baseline load tests for portal queries, import pipelines, sync processing.
- Define SLO targets and regression thresholds.

Acceptance criteria:
- [x] Baseline reports committed for key flows.
- [x] Threshold breach fails pipeline or marks release as blocked.

## 4) Production Readiness Checklist (Pass/Fail)

Use this as the release gate checklist. Mark `PASS` only with linked evidence (PR, test run, metric snapshot).

| Checkpoint | Priority | Owner | Status (PASS/FAIL) | Evidence |
|---|---|---|---|---|
| PR-P0-01 Authorization boundary hardening complete | P0 | Backend | PASS | Policy/controller updates + `ApiAuthorizationPoliciesTests` |
| PR-P0-02 Soft delete filter integrity complete | P0 | Backend | PASS | `SoftDeleteQueryFilterTests` |
| PR-P0-03 User role change safety fix complete | P0 | Backend | PASS | `ChangeUserRoleCommandHandlerTests` |
| PR-P0-04 Blob storage config fail-safe complete | P0 | Backend | PASS | `BlobStorageServiceTests` |
| PR-P0-05 Exception sanitization complete | P0 | Backend | PASS | `ApiControllerBaseErrorSanitizationTests` |
| PR-P1-01 Query efficiency/pagination sweep complete | P1 | Backend | PASS | Visit/site/report/admin query handlers now use spec-based DB filtering and bounded pagination; coverage in `VisitQueryEfficiencyTests`, `ReportQueryEfficiencyTests`, and `AdminListQueryEfficiencyTests` |
| PR-P1-02 Import guardrails complete | P1 | Backend | PASS | `ImportGuardrails` + handler enforcement (`Import:MaxRows`, `Import:SkipInvalidRows`, file type/size); tests: `ImportSiteDataCommandHandlerTests`, `ImportCommandsRealFilesIntegrationTests`, `Sprint12DryRunReconciliationTests` |
| PR-P1-03 HTTP security controls complete | P1 | Platform | PASS | Path-scoped rate limiting (`/api/auth/*`, `/api/sync*`, `*/import*`), production HSTS enabled, strict production CORS validation; tests: `ApiSecurityHardeningTests` |
| PR-P1-04 Observability completion complete | P1 | Platform | PASS | Correlation ID middleware (`X-Correlation-ID`) + scoped request logs; `IOperationalMetrics` instrumentation for import/sync/notification flows; runbook: `docs/phase-2/13-observability-runbook.md`; tests: `CorrelationIdMiddlewareTests`, `OperationalMetricsBehaviorTests`, `NotificationServiceTests` |
| PR-P2-01 Error contract unification complete | P2 | Backend | PASS | Unified API error schema (`Code`, `Message`, `CorrelationId`, optional `Errors`/`Meta`) implemented in `ApiControllerBase`, `ExceptionHandlingMiddleware`, and validation filter; tests: `ApiControllerBaseErrorSanitizationTests`, `ExceptionHandlingMiddlewareLocalizationTests` |
| PR-P2-02 Domain/docs drift automation complete | P2 | Backend | PASS | `tools/check_doc_drift.py` now validates controller coverage + policy coverage + critical command (import/export/audit/customer acceptance) coverage across `Api-Doc` and `Application-Doc` |
| PR-P2-03 Performance baseline complete | P2 | QA/Platform | PASS | Baseline thresholds enforced by `PerformanceBaselineSmokeTests`; report committed at `docs/phase-2/14-performance-baseline.md`; latest run: 3/3 pass |
| `dotnet build TowerOps.sln` green on CI | Gate | CI | PASS | GitHub Actions `.NET CI` run #90 (success): `https://github.com/<owner>/<repo>/actions/runs/22373242789` |
| `dotnet test TowerOps.sln --logger "console;verbosity=minimal"` green on CI | Gate | CI | PASS | GitHub Actions `.NET CI` run #90 (success): `https://github.com/<owner>/<repo>/actions/runs/22373242789` |
| `python tools/check_doc_drift.py` green on CI | Gate | CI | PASS | GitHub Actions `.NET CI` run #90 includes doc drift check and passed: `https://github.com/<owner>/<repo>/actions/runs/22373242789` |
| Staging smoke test (Auth, Visits, WorkOrders, Import, Portal) executed | Gate | QA | PASS | `docs/phase-2/15-staging-smoke-and-rollback-verification.md` (92 tests across Auth/Visits/WorkOrders/Import/Portal) |
| Rollback and migration verification signed off | Gate | Backend/DBA | PASS | `docs/phase-2/15-staging-smoke-and-rollback-verification.md` + EF rehearsal (latest apply -> latest rollback -> DB drop) |

## 5) Suggested Delivery Sequence

1. Execute all **P0 PRs** first, each independently mergeable and test-backed.
2. Execute **P1** in parallel tracks:
   - Track A: Query/import hardening (`P1-01`, `P1-02`)
   - Track B: Platform hardening (`P1-03`, `P1-04`)
3. Execute **P2** and convert to continuous quality gates.

## 6) Exit Condition

Production-ready declaration requires:
- All **P0** items `PASS`.
- At least **P1-01/P1-02/P1-03** `PASS`.
- CI gates all green.
- Staging smoke test + rollback rehearsal completed with evidence links.
