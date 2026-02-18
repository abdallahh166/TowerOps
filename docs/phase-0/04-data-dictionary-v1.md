# Phase 0 — Data Dictionary v1 (Starter Template)

> This starter template should be converted to governed Excel/DB dictionary during Phase 1.

## Canonical Keys (Mandatory)
- `OfficeCode`
- `SiteCode`
- `VisitId`
- `CapturedAt`

## Dictionary Columns
| FieldName | Business Definition | Data Type | Allowed Values | Required | Source Sheet/System | Owner | Notes |
|---|---|---|---|---|---|---|---|
| OfficeCode | Unique office identifier | string | Org-defined code list | Yes | All templates/system | Operations | Canonical key |
| SiteCode | Unique telecom site identifier | string | Site registry values | Yes | All templates/system | Operations | Canonical key |
| VisitId | Unique visit execution identifier | guid/string | System generated | Yes | Visit workflow | Engineering | Canonical key |
| CapturedAt | Timestamp of captured evidence/record | datetime (UTC) | ISO-8601 | Yes | Evidence/Readings | Engineering | Canonical key |
| WoNumber | Unique work order number | string | WO format policy | Yes | WO lifecycle | Dispatch | |
| SlaClass | SLA priority class | enum | P1,P2,P3,P4 | Yes | WO lifecycle | Operations | |
| ApprovalStage | Current approval stage | enum | CM/BM stage values | Yes | Approval engine | QA | |
| EvidenceCompleteness | Completion score for evidence package | decimal(0-100) | 0..100 | No | KPI pipeline | QA | Derived field |

## Enum Catalog (v1)
### SLA Class
- `P1`
- `P2`
- `P3`
- `P4`

### Checklist Status
- `OK`
- `NOK`
- `NA`

### Work Order State (initial)
- `Created`
- `Assigned`
- `InProgress`
- `PendingReview`
- `PendingCustomerAcceptance`
- `Closed`
- `Rejected`
- `Rework`
- `Cancelled`
