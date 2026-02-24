# Phase 1 - Use Cases (Historical Design Baseline)

This file preserves the high-level design baseline. Actual behavior may be stricter or extended in code.

## UC-01 Create Work Order
- Actor: dispatcher/coordinator
- Preconditions: site exists; SLA class selected
- Flow:
  1. Create work order with issue and SLA context.
  2. System stores UTC timestamps and initial status.
  3. System validates required fields.
- Postcondition: work order available for assignment.

## UC-02 Assign Work Order
- Actor: dispatcher/coordinator
- Preconditions: work order in assignable status
- Flow:
  1. Assign engineer and office context.
  2. System updates status and assignment metadata.
- Postcondition: engineer receives assignment.

## UC-03 Execute Visit
- Actor: field engineer
- Preconditions: visit scheduled/assigned
- Flow:
  1. Start visit (and optional check-in).
  2. Capture readings/checklist/photos/issues/material usage.
  3. Complete and submit for review.
- Postcondition: visit enters review workflow.

## UC-04 Evidence Submission
- Actor: field engineer
- Preconditions: visit in submit-eligible state
- Flow:
  1. Attach evidence package.
  2. Evidence policy validation is enforced.
- Postcondition: review-ready submission.

## UC-05 Review and Decision
- Actor: reviewer/manager
- Preconditions: submitted visit/work item
- Flow:
  1. Review submitted evidence.
  2. Approve/reject/request correction.
  3. System stores decision history.
- Postcondition: approved closure or rework loop.

## UC-06 Escalation
- Actor: authorized escalation approver path
- Preconditions: escalation trigger and required data
- Flow:
  1. Submit escalation.
  2. Review and decide.
  3. Close escalation.
- Postcondition: escalation lifecycle auditable.

## UC-07 Close Work Order and KPI Impact
- Actor: authorized manager/customer flow
- Preconditions: completion/review/customer acceptance satisfied
- Flow:
  1. Transition to closed state.
  2. KPI and SLA projections include new state.
- Postcondition: immutable closure state until explicit rework path.
