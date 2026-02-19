# Sprint 3 — Delivery Contract (Execution)

## Scope
1. Escalation API with hard mandatory-field validation
2. Escalation tracking endpoint
3. Role-based access policies for escalation management/view
4. Persistence model for escalation audit trail
5. Review policy engine for CM/BM visit approvals
6. Reject/request-correction rework loop support for visit resubmission
7. Escalation routing rule enforcement

## Delivered API Endpoints
- `POST /api/escalations`
- `GET /api/escalations/{escalationId}`

## Acceptance Criteria
- Escalation cannot be created without required fields (incident/site/evidence/actions/recommendation/submittedBy).
- Duplicate incident escalation is rejected.
- Escalation management is restricted by authorization policy.
- Escalation lifecycle starts in `Submitted` state with UTC timestamp.
- Visit review actions are policy-driven (CM/BM path) instead of hardcoded role checks.
- Visit in `NeedsCorrection` can be resubmitted after rework.
- Escalation level must match routing rule output.

## Test Matrix
1. Create escalation happy path
2. Create escalation with missing evidence (validation/domain failure)
3. Duplicate incident escalation (business failure)
4. Get escalation by id

## Next Handoff
- Sprint 4: SLA breach engine + KPI endpoints + release hardening.


## Status Update
- ✅ Escalation endpoints implemented in `EscalationsController`.
- ✅ Escalation authorization policies now wired in API startup via `ApiAuthorizationPolicies`.
- ✅ Visit review actions use `CanReviewVisits` policy.

## Implementation References
- `src/TelecomPm.Api/Controllers/EscalationsController.cs`
- `src/TelecomPm.Api/Controllers/VisitsController.cs`
- `src/TelecomPm.Api/Authorization/ApiAuthorizationPolicies.cs`
