# Business Confirmation Implementation PR Slices

## Scope
This plan converts approved business confirmations into executable backend PR slices.

Source decisions:
- `docs/Business-Confirmation-Checklist.md`

## Priority
- P0: BC-01, BC-02, BC-03, BC-04, BC-07
- P1: BC-05, BC-06, BC-08 (implemented; keep for traceability)

## PR-Slice Plan

## PR-BC-01 Auth Session Model
Issue: [#47](https://github.com/abdallahh166/TowerOps/issues/47)  
Goal:
- Implement access token 15m + rotating refresh token 7d with server-side revocation.

Deliverables:
- Refresh token entity/repository and persistence model
- Refresh endpoint and rotation logic
- Logout revocation path
- Password-change global revocation path
- Tests for rotate/revoke/logout/password-change flows

Acceptance Criteria:
- Access token expiry is 15 minutes in runtime config.
- Refresh token expiry is 7 days.
- Reused/rotated refresh token is rejected.
- Password change invalidates all user refresh tokens.

## PR-BC-02 Account Protection and MFA
Issue: [#48](https://github.com/abdallahh166/TowerOps/issues/48)  
Goal:
- Progressive lockout + role-based MFA requirement.

Deliverables:
- Failed login counter + lockout windows
- 3rd lockout in 24h requires manual admin unlock
- Admin notification on lock
- TOTP MFA for Admin and Manager
- Optional MFA enrollment for Engineer/Viewer
- Tests for lockout progression and MFA policy enforcement

Acceptance Criteria:
- 5 failed attempts trigger 15-minute lock.
- Third lockout in 24h enters manual unlock state.
- Admin/Manager login without MFA is denied.

## PR-BC-03 Portal Workorder Permission Split
Issue: [#49](https://github.com/abdallahh166/TowerOps/issues/49)  
Goal:
- Split view and mutation permissions for portal workorders.

Deliverables:
- Add `portal.manage_workorders` permission constant
- Update `CanManagePortalWorkOrders` policy
- Update portal accept/reject endpoints authorization
- Update role defaults/seeding for new portal users (view-only)
- Policy tests

Acceptance Criteria:
- Users with only `portal.view_workorders` cannot mutate.
- Users with `portal.manage_workorders` can accept/reject.

## PR-BC-04 Error Contract Unification
Issue: [#50](https://github.com/abdallahh166/TowerOps/issues/50)  
Goal:
- Enforce one standardized error envelope (RFC 9457 ProblemDetails-aligned) on all endpoints.

Deliverables:
- Centralized middleware mapping for all exception categories
- Remove controller-level raw-string error responses
- Update docs examples to single error format
- Add integration tests for envelope consistency

Acceptance Criteria:
- No endpoint returns plain-string errors.
- All 4xx/5xx responses include consistent structured error fields.

## PR-BC-07 Unified Pagination and Sorting
Issue: [#53](https://github.com/abdallahh166/TowerOps/issues/53)  
Goal:
- Enforce one list contract before frontend list components are finalized.

Deliverables:
- Shared query input model: `page`, `pageSize`, `sortBy`, `sortDir`
- Shared paginated response envelope:
  - `data`
  - `pagination` (`page`, `pageSize`, `total`, `totalPages`, `hasNextPage`, `hasPreviousPage`)
- Per-endpoint server-side `sortBy` allowlists
- `400` response for unknown sort field
- Contract tests

Acceptance Criteria:
- All list endpoints return identical pagination structure.
- Unknown `sortBy` returns validation error.

## P1 Delivery (Post-Decision Execution)

## BC-05 Data Retention and Privacy
Issue: [#51](https://github.com/abdallahh166/TowerOps/issues/51)  
Status: implemented and merged in [#68](https://github.com/abdallahh166/TowerOps/pull/68). Delivery tracking issue [#62](https://github.com/abdallahh166/TowerOps/issues/62) is closed.

## BC-06 Upload Security Policy
Issue: [#52](https://github.com/abdallahh166/TowerOps/issues/52)  
Status: implemented and merged.

## BC-08 SLA Definition Finalization
Issue: [#54](https://github.com/abdallahh166/TowerOps/issues/54)  
Status: implemented and merged.

## Execution Order
1. PR-BC-03 (low-risk permission split)
2. PR-BC-04 (error contract consistency)
3. PR-BC-07 (pagination/sort contract)
4. PR-BC-01 (session/token architecture)
5. PR-BC-02 (lockout + MFA)

Rationale:
- Start with low blast-radius authorization and contract alignment.
- Then ship auth/security model changes with focused testing and migration windows.
