# Phase 2 Follow-up Ticket Drafts

Date: 2026-02-25  
Owner: Backend / DBA

Use these drafts to create the two non-blocker follow-up issues referenced in phase-2 release gates.

## Ticket 1: EF Enum Sentinel Warnings

Title:
`P1: EF enum sentinel warnings hardening`

Labels:
`tech-debt`, `ef-core`, `post-release`

Body:
```markdown
## Problem
EF warns about enum sentinel/default behavior for:
- `Site.ResponsibilityScope`
- `Site.TowerOwnershipType`
- `WorkOrder.Scope`

## Risk
Ambiguous default/sentinel behavior can create subtle data-shape bugs and make future migrations fragile.

## Scope
1. Add explicit EF configuration for enum defaults/sentinels.
2. Ensure migration is additive and backward-safe.
3. Add regression tests for create/read/update with default enum values.

## Acceptance Criteria
- No enum sentinel warnings during migration/update.
- Existing records remain readable without data loss.
- Tests cover default-value behavior and pass in CI.
```

## Ticket 2: Escalation Decimal Precision

Title:
`P1: configure explicit decimal precision for escalation fields`

Labels:
`tech-debt`, `ef-core`, `post-release`

Body:
```markdown
## Problem
EF warns that escalation decimal precision is not explicitly configured for:
- `Escalation.FinancialImpactEgp`
- `Escalation.SlaImpactPercentage`

## Risk
Implicit precision can cause truncation/rounding differences across environments.

## Scope
1. Add explicit precision/scale in EF configuration.
2. Generate additive migration.
3. Add repository/integration test covering persisted precision.

## Acceptance Criteria
- No decimal precision warnings for escalation fields.
- Persisted values match expected precision/scale in SQL Server.
- Migration applies cleanly in staging and CI.
```

## Optional GitHub CLI Commands

If `gh` is installed/authenticated:

```bash
gh issue create --title "P1: EF enum sentinel warnings hardening" --body-file docs/phase-2/18-follow-up-ticket-drafts.md --label tech-debt,ef-core,post-release
gh issue create --title "P1: configure explicit decimal precision for escalation fields" --body-file docs/phase-2/18-follow-up-ticket-drafts.md --label tech-debt,ef-core,post-release
```

## GitHub Actions Automation (recommended)

If `gh` CLI is unavailable locally, trigger:

- Workflow: `.github/workflows/create-followup-issues.yml`
- Action used: `actions/github-script@v7`
- Behavior: idempotent (reuses existing open issues by title)
