# Phase 1 — Use Cases (v1)

## UC-01 Create Work Order
- **Actor:** Dispatcher / Coordinator
- **Preconditions:** Site exists, SLA class selectable (P1..P4)
- **Main Flow:**
  1. User creates WO with site, issue type, severity, and SLA class.
  2. System timestamps in UTC and sets status `Created`.
  3. System validates mandatory fields and stores audit record.
- **Postconditions:** WO available for assignment.

## UC-02 Assign / Dispatch Work Order
- **Actor:** Dispatcher / Coordinator
- **Preconditions:** WO in `Created`
- **Main Flow:**
  1. Assign engineer/team and office context.
  2. System sets `Assigned` and starts response SLA clock.
- **Postconditions:** Execution team notified.

## UC-03 Execute Visit
- **Actor:** Hardware Engineer (CM) / BM Field Engineer
- **Preconditions:** WO assigned
- **Main Flow:**
  1. Engineer starts visit with `CapturedAt` UTC.
  2. Engineer records readings/checklist/actions.
  3. System stores evidence metadata.
- **Postconditions:** Visit moves to `InProgress`/`PendingReview`.

## UC-04 Submit Evidence Package
- **Actor:** Field Engineer
- **Preconditions:** Visit execution complete
- **Main Flow:**
  1. Upload before/after photos, checklist, readings, and notes.
  2. System validates completeness policy.
- **Postconditions:** Ready for review.

## UC-05 Review / Approve / Reject / Rework
- **Actors:** CM Supervisor, Office Manager, BM Monitoring Engineer, BM Manager
- **Preconditions:** Evidence package submitted
- **Main Flow:**
  1. Reviewer validates checklist and evidence.
  2. Approve or reject with reason.
  3. Rejection sets `Rework` and preserves SLA/audit history.
- **Postconditions:** Progresses to final acceptance or rework loop.

## UC-06 Escalation Management
- **Actors:** CM Supervisor, Area Manager, BM Manager, Sponsor (exception)
- **Preconditions:** Escalation trigger met
- **Main Flow:**
  1. User submits escalation request.
  2. System blocks submission unless required escalation fields are complete.
  3. Escalation routed by policy and severity.
- **Postconditions:** Escalation state and owner updated.

## UC-07 Close Work Order + SLA Evaluation
- **Actor:** Final Approver
- **Preconditions:** Final approval granted
- **Main Flow:**
  1. System closes WO.
  2. System computes SLA compliance, breach flags, MTTR, FTF contribution.
- **Postconditions:** WO immutable for closure audit except formal reopen.
