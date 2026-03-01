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
- `Planned`: approved and scoped for implementation, not delivered yet

## Decision Tracker
| ID | Topic | Decision Required | Impact | Owner | Due Date | Status | Decision Notes | Linked Ticket |
|---|---|---|---|---|---|---|---|---|
| BC-01 | Session policy | Confirm refresh-token/revocation/forced logout/device session behavior | High | Abdullah Mahmoud (Product + Backend) | 2026-03-03 | Approved | Access token TTL: 15 minutes. Refresh token TTL: 7 days, rotating on each use. Server-side revocation list in Redis/DB. On password change: revoke ALL refresh tokens for that user immediately. On explicit logout: revoke the presented refresh token. | [#47](https://github.com/abdallahh166/TowerOps/issues/47) |
| BC-02 | Account protection | Confirm lockout thresholds, brute-force controls, MFA requirements by role | High | Security Owner (interim: Abdullah Mahmoud) | 2026-03-03 | Approved | Failed login lockout: 5 attempts -> 15-minute auto-unlock. Third lockout within 24 hours -> admin manual unlock. Admin receives account-lock notification. MFA (TOTP) mandatory for Admin and Manager; optional for Engineer and Viewer. | [#48](https://github.com/abdallahh166/TowerOps/issues/48) |
| BC-03 | Portal workorder permission | Confirm whether `portal.view_workorders` is sufficient for accept/reject mutations | High | Product Owner + Backend Lead | 2026-03-02 | Approved | Split permissions: `portal.view_workorders` (read-only) and `portal.manage_workorders` (accept/reject/request-revision). New portal users default to view-only; manage permission must be explicitly granted by system admin. | [#49](https://github.com/abdallahh166/TowerOps/issues/49) |
| BC-04 | Success envelope standard | Confirm if a global success wrapper is required across all APIs | Medium | Frontend Lead + Backend Lead | 2026-03-04 | Approved | Decision closed: reject draft "raw-success now, wrapper in v2". Enforce RFC 9457 ProblemDetails-style error envelope on all endpoints now. No plain-string errors. Keep success as standard HTTP 200/201/204 with typed body or empty body (no success wrapper required). | [#50](https://github.com/abdallahh166/TowerOps/issues/50) |
| BC-05 | Data retention/privacy | Confirm retention/deletion/export rules for photos, signatures, audits | High | Product Owner + Legal/Compliance | 2026-03-05 | Approved | Operational data retained 5 years from creation; signatures retained 7 years; audit logs immutable for 7 years and exempt from deletion requests. User/photo soft-delete with 90-day grace before hard purge. Export API allows authenticated users to request JSON export of their own operational data within 30 days. Legal hold blocks purge. Jurisdiction uses strictest applicable law among active client contracts. | Decision: [#51](https://github.com/abdallahh166/TowerOps/issues/51), Delivery: [#58](https://github.com/abdallahh166/TowerOps/issues/58) |
| BC-06 | Upload security | Confirm malware scanning/content sanitization policy for uploaded files | High | Platform/Security Lead | 2026-03-04 | Approved | All uploads land in quarantine storage bucket first. Scan within 60 seconds via ClamAV (self-hosted) or Azure Defender for Storage. File status lifecycle: Pending -> Approved -> Quarantined. Portal/reports serve only Approved files. Engineer upload UX is optimistic with Pending indicator. On quarantine: notify platform admin and engineer; block serving. Enforce magic-byte validation + extension allowlist (.jpg, .jpeg, .png, .pdf) before quarantine. No content disarm in v1. | Decision: [#52](https://github.com/abdallahh166/TowerOps/issues/52), Delivery: [#59](https://github.com/abdallahh166/TowerOps/issues/59) |
| BC-07 | Pagination standard | Confirm mandatory unified pagination contract for all list endpoints | Medium | Frontend Lead + Backend Lead | 2026-03-01 | Approved | Unified contract before frontend list components: request `page` (default 1), `pageSize` (default 25, max 100), `sortBy` (server allowlist), `sortDir` (`asc|desc`, default `desc`). Response `{ data, pagination: { page, pageSize, total, totalPages, hasNextPage, hasPreviousPage } }`. Reject unknown `sortBy` with 400. | [#53](https://github.com/abdallahh166/TowerOps/issues/53) |
| BC-08 | SLA business definitions | Confirm final definitions for at-risk/breach/compliance cutoff logic | High | Operations Manager + Product Owner | 2026-03-05 | Approved | CM SLA clock starts at work-order creation timestamp (server UTC). PM SLA clock starts at scheduled visit datetime configured on work order. At-risk threshold configurable per work-order type in SystemSettings (default 80% of target window elapsed). Breach threshold is 100%. SLA calculations use server UTC only. Engineer-reported completion time is metadata for reporting and does not override server SLA status. Reporting uses UTC internally with display conversion to client timezone. SLA target hours per work-order type must be configured in SystemSettings from operations contract values. | Decision: [#54](https://github.com/abdallahh166/TowerOps/issues/54), Delivery: [#60](https://github.com/abdallahh166/TowerOps/issues/60) |

## Decision Session Plan (Proposed)
1. Session A (Completed): BC-01, BC-02, BC-03, BC-04, BC-07.
2. Session B (Completed): BC-05, BC-06, BC-08.

## Implementation Tracker
| ID | Delivery Status | GitHub Issue | Implementation PR | Notes |
|---|---|---|---|---|
| BC-01 | Implemented | Closed `#47` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Refresh token rotation, revoke-on-logout, revoke-all on password change, 15m/7d policy applied |
| BC-02 | Implemented | Closed `#48` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Progressive lockout, manual admin unlock endpoint, MFA setup/verify + Admin/Manager enforcement |
| BC-03 | Implemented | Closed `#49` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | `portal.view_workorders` vs `portal.manage_workorders` split enforced |
| BC-04 | Implemented | Closed `#50` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Standardized structured error envelope across API responses |
| BC-07 | Implemented | Closed `#53` | [#56](https://github.com/abdallahh166/TowerOps/pull/56) | Unified pagination/sort contract and sort-field allowlists |
| BC-05 | Planned | Open `#62` | TBD | Decision approved. Pending implementation (retention, legal hold, export flow). Supersedes delivery tracking from `#58`. |
| BC-06 | In Delivery | Open `#64` | TBD (`feat/bc-06-quarantine-upload`) | Quarantine-first upload pipeline, magic-byte validation, malware scan worker, and approved-only portal/report serving implemented on feature branch; pending PR merge. |
| BC-08 | Implemented | Closed `#63` | [#65](https://github.com/abdallahh166/TowerOps/pull/65) | Implemented and merged: CM/PM SLA start rules, type-based at-risk thresholds, UTC authority, engineer completion metadata, and migrations. |

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
- BC-05/06/08 are approved and tracked to delivery with dedicated implementation issues/PRs.
