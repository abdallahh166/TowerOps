# Phase 2 Definition of Done (DoD) Checklist

Status: **Completed** (release blockers closed in code, tests, docs, and handoff package).

Each sprint deliverable must satisfy all checks before closure:

## 1) Policy & Security
- [x] Required endpoint policies are defined in `ApiAuthorizationPolicies`.
- [x] Endpoints have `[Authorize(Policy=...)]` where required.
- [x] Positive + negative authorization tests exist.

## 2) Tests
- [x] Domain rules covered (happy path + invalid transitions).
- [x] Application service/handler tests updated.
- [x] Infrastructure service tests cover external provider calls and fallback behavior.
- [x] No active `NotImplementedException` on production execution path.
- [x] Real Excel integration tests exist for sprint import/export handlers using files from `docs/excell`.

## 3) Documentation
- [x] `docs/Api-Doc.md` updated with endpoint and policy changes.
- [x] Sprint delivery contract updated with status + references + payload samples.
- [x] Business/operating docs updated for capability changes.
- [x] Dry-run reconciliation report captured (imported/skipped/errors/entity diff) for Sprint 12.

## 4) Telemetry & Operations
- [x] Key actions are logged with correlation context.
- [x] Error paths provide actionable diagnostics.
- [x] Health-check and runtime configuration impacts documented.

## 5) Handoff
- [x] PR includes validation commands and outcomes.
- [x] Release notes summarize behavior changes and migration impact.

## Closure Evidence
- Policy wiring + endpoint protection: `src/TelecomPm.Api/Authorization/ApiAuthorizationPolicies.cs`, controller policy attributes.
- Unified exception handling path: `src/TelecomPm.Api/Middleware/ExceptionHandlingMiddleware.cs`.
- API-edge mapping strategy rollout: `src/TelecomPm.Api/Mappings/*.cs` and mapped controllers.
- Shared editable visit mutation workflow: `src/TelecomPm.Application/Services/EditableVisitMutationService.cs`.
- Test coverage additions: mapper tests, auth/policy tests, infrastructure notification/report tests.
- Release notes: `docs/phase-2/08-release-readiness-report.md`.
- Sprint 12 reconciliation: `docs/phase-2/10-sprint-12-dry-run-reconciliation.md`.
