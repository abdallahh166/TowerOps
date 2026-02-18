# Phase 1 — API Contract Draft v1

## Work Orders
- `POST /api/workorders`
  - Create WO (site, severity, SLA class, issue context)
- `GET /api/workorders/{woId}`
  - Get WO details + SLA state + approval stage
- `POST /api/workorders/{woId}/assign`
  - Assign engineer/team
- `POST /api/workorders/{woId}/close`
  - Close WO (final gate)

## Visits & Evidence
- `POST /api/workorders/{woId}/visits/start`
- `POST /api/workorders/{woId}/visits/{visitId}/readings`
- `POST /api/workorders/{woId}/visits/{visitId}/checklist`
- `POST /api/workorders/{woId}/visits/{visitId}/photos`
- `POST /api/workorders/{woId}/visits/{visitId}/submit`

## Approval Flow
- `POST /api/workorders/{woId}/approvals/review`
- `POST /api/workorders/{woId}/approvals/approve`
- `POST /api/workorders/{woId}/approvals/reject`
- `POST /api/workorders/{woId}/approvals/rework`

## Escalations
- `POST /api/workorders/{woId}/escalations`
  - Hard validation required:
    - incidentId, siteCode, severity, financialEstimate,
    - slaImpactPercent, evidencePackage, previousActions, recommendedDecision
- `GET /api/workorders/{woId}/escalations`

## Dashboards
- `GET /api/dashboard/kpis?workflowType=CM|BM&area=&office=&priority=&slaStatus=&approvalStage=`
- `GET /api/dashboard/breaches?from=&to=&severity=`

## Security/Policy Notes
- All endpoints policy-protected by role + stage.
- Timestamps persisted in UTC and localized in presentation layer.
- Approval chains are configuration-driven (no hardcoded sequence).
