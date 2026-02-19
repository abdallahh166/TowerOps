# Sprint 2 — Delivery Contract (Execution)

## Scope
1. Visit start / complete / submit workflow
2. Evidence capture APIs (photos, readings, checklist)
3. Issue capture and issue resolution APIs
4. Evidence completeness snapshot endpoint

## Delivered API Endpoints
- `GET /api/visits/{visitId}/evidence-status`
- `POST /api/visits/{visitId}/photos`
- `POST /api/visits/{visitId}/readings`
- `POST /api/visits/{visitId}/checklist-items`
- `PATCH /api/visits/{visitId}/checklist-items/{checklistItemId}`
- `POST /api/visits/{visitId}/issues`
- `POST /api/visits/{visitId}/issues/{issueId}/resolve`

## Acceptance Criteria
- Evidence completeness must be queryable in one API response.
- Checklist updates must affect visit completion score.
- Issue logging and resolution are both supported through API.
- Visit submission can be monitored via `CanBeSubmitted` in evidence status.

## Test Matrix
1. Add checklist item (happy path)
2. Update checklist item status
3. Add issue and resolve issue
4. Evidence-status query for existing visit
5. Evidence-status query for missing visit

## Next Handoff
- Sprint 3: Approval policy engine + escalation rules execution.


## Sample Payloads
`POST /api/visits/{visitId}/checklist-items`
```json
{
  "category": "Power",
  "itemName": "Rectifier Visual Check",
  "description": "Check wiring and alarms",
  "isMandatory": true
}
```

`POST /api/visits/{visitId}/issues/{issueId}/resolve`
```json
{
  "resolution": "Replaced damaged cable lug",
  "resolvedBy": "engineer@telecompm.com"
}
```

## Implementation References
- Controller: `src/TelecomPm.Api/Controllers/VisitsController.cs`
- Query handler: `GetVisitEvidenceStatusQueryHandler`
