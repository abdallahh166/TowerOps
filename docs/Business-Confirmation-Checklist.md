# Business Confirmation Checklist

## Purpose
This checklist tracks business decisions that cannot be finalized from code discovery alone.

Source:
- `docs/Backend-Auto-Discovery-Report.md` (`Business clarification required`)

## Status Legend
- `Open`: not discussed yet
- `In Review`: discussion started
- `Approved`: business approved and documented
- `Rejected`: explicitly declined
- `Implemented`: approved and delivered in code (PR merged)
- `In Delivery`: approved and implemented in PR, pending merge checks

## Decision Tracker
| ID | Topic | Decision Required | Impact | Owner | Due Date | Status | Decision Notes | Linked Ticket |
|---|---|---|---|---|---|---|---|---|
| BC-01 | Session policy | Confirm refresh-token/revocation/forced logout/device session behavior | High | Abdullah Mahmoud (Product + Backend) | 2026-03-03 | Approved | Access token TTL: 15 minutes. Refresh token TTL: 7 days, rotating on each use. Server-side revocation list in Redis/DB. On password change: revoke ALL refresh tokens for that user immediately. On explicit logout: revoke the presented refresh token. | [#47](https://github.com/abdallahh166/TowerOps/issues/47) |
| BC-02 | Account protection | Confirm lockout thresholds, brute-force controls, MFA requirements by role | High | Security Owner (interim: Abdullah Mahmoud) | 2026-03-03 | Approved | Failed login lockout: 5 attempts -> 15-minute auto-unlock. Third lockout within 24 hours -> admin manual unlock. Admin receives account-lock notification. MFA (TOTP) mandatory for Admin and Manager; optional for Engineer and Viewer. | [#48](https://github.com/abdallahh166/TowerOps/issues/48) |
| BC-03 | Portal workorder permission | Confirm whether `portal.view_workorders` is sufficient for accept/reject mutations | High | Product Owner + Backend Lead | 2026-03-02 | Approved | Split permissions: `portal.view_workorders` (read-only) and `portal.manage_workorders` (accept/reject/request-revision). New portal users default to view-only; manage permission must be explicitly granted by system admin. | [#49](https://github.com/abdallahh166/TowerOps/issues/49) |
| BC-04 | Success envelope standard | Confirm if a global success wrapper is required across all APIs | Medium | Frontend Lead + Backend Lead | 2026-03-04 | Approved | Decision closed: reject draft "raw-success now, wrapper in v2". Enforce RFC 9457 ProblemDetails-style error envelope on all endpoints now. No plain-string errors. Keep success as standard HTTP 200/201/204 with typed body or empty body (no success wrapper required). | [#50](https://github.com/abdallahh166/TowerOps/issues/50) |
| BC-05 | Data retention/privacy | Confirm retention/deletion/export rules for photos, signatures, audits | High | Product Owner + Legal/Compliance | 2026-03-05 | Open |  | [#51](https://github.com/abdallahh166/TowerOps/issues/51) |
| BC-06 | Upload security | Confirm malware scanning/content sanitization policy for uploaded files | High | Platform/Security Lead | 2026-03-04 | Open |  | [#52](https://github.com/abdallahh166/TowerOps/issues/52) |
| BC-07 | Pagination standard | Confirm mandatory unified pagination contract for all list endpoints | Medium | Frontend Lead + Backend Lead | 2026-03-01 | Approved | Unified contract before frontend list components: request `page` (default 1), `pageSize` (default 25, max 100), `sortBy` (server allowlist), `sortDir` (`asc|desc`, default `desc`). Response `{ data, pagination: { page, pageSize, total, totalPages, hasNextPage, hasPreviousPage } }`. Reject unknown `sortBy` with 400. | [#53](https://github.com/abdallahh166/TowerOps/issues/53) |
| BC-08 | SLA business definitions | Confirm final definitions for at-risk/breach/compliance cutoff logic | High | Operations Manager + Product Owner | 2026-03-05 | Open |  | [#54](https://github.com/abdallahh166/TowerOps/issues/54) |

## Decision Session Plan (Proposed)
1. Session A (Completed): BC-01, BC-02, BC-03, BC-04, BC-07.
2. Session B (2026-03-05): BC-05, BC-06, BC-08.

## Implementation Tracker
| ID | Delivery Status | GitHub Issue | Implementation PR | Notes |
|---|---|---|---|---|
| BC-01 | In Delivery | Closed `#47` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Refresh token rotation, revoke-on-logout, revoke-all on password change, 15m/7d policy applied |
| BC-02 | In Delivery | Closed `#48` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Progressive lockout, manual admin unlock endpoint, MFA setup/verify + Admin/Manager enforcement |
| BC-03 | In Delivery | Closed `#49` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | `portal.view_workorders` vs `portal.manage_workorders` split enforced |
| BC-04 | In Delivery | Closed `#50` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Standardized structured error envelope across API responses |
| BC-07 | In Delivery | Closed `#53` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Unified pagination/sort contract and sort-field allowlists |
| BC-05 | Open | Open `#51` | TBD | Awaiting legal/compliance decision |
| BC-06 | Open | Open `#52` | TBD | Awaiting platform/security decision |
| BC-08 | Open | Open `#54` | TBD | Awaiting operations/product decision |

## After Each Decision
1. Update `Status` to `Approved` or `Rejected`.
2. Write final wording in `Decision Notes`.
3. Replace placeholder `Linked Ticket` with real issue/PR id.
4. Reflect approved decisions in:
   - `docs/Api-Doc.md`
   - `docs/Application-Doc.md`
   - `docs/Domain-Doc.md`
   - `docs/Documentation-Gap-Report.md`

## Completion Criteria
- All `BC-*` items are `Approved` or `Rejected`.
- Approved decisions are reflected in:
  - `docs/Api-Doc.md`
  - `docs/Application-Doc.md`
  - `docs/Domain-Doc.md`
  - `docs/Documentation-Gap-Report.md`
- Related implementation tickets are linked and prioritized.
- BC-01/02/03/04/07 move from `In Delivery` to `Implemented` once PR `#56` is merged.
