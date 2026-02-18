# Phase 2 Definition of Done (DoD) Checklist

Each sprint deliverable must satisfy all checks before closure:

## 1) Policy & Security
- [ ] Required endpoint policies are defined in `ApiAuthorizationPolicies`.
- [ ] Endpoints have `[Authorize(Policy=...)]` where required.
- [ ] Positive + negative authorization tests exist.

## 2) Tests
- [ ] Domain rules covered (happy path + invalid transitions).
- [ ] Application service/handler tests updated.
- [ ] Infrastructure service tests cover external provider calls and fallback behavior.
- [ ] No active `NotImplementedException` on production execution path.

## 3) Documentation
- [ ] `docs/Api-Doc.md` updated with endpoint and policy changes.
- [ ] Sprint delivery contract updated with status + references + payload samples.
- [ ] Business/operating docs updated for capability changes.

## 4) Telemetry & Operations
- [ ] Key actions are logged with correlation context.
- [ ] Error paths provide actionable diagnostics.
- [ ] Health-check and runtime configuration impacts documented.

## 5) Handoff
- [ ] PR includes validation commands and outcomes.
- [ ] Release notes summarize behavior changes and migration impact.
