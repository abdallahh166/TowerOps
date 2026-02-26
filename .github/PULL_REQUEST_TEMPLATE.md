
## Summary
- What changed?
- Why was this needed?

## Scope
- [ ] Backend
- [ ] Frontend
- [ ] CI/CD
- [ ] Docs

## Linked Issue
- Closes #

## API / Contract Impact
- [ ] No API changes
- [ ] Backward-compatible API change
- [ ] Breaking API change (migration plan included)

## Test Evidence
- [ ] Unit tests updated/added
- [ ] Integration tests updated/added
- [ ] Manual smoke test completed

Notes:
- Commands run:
  - `dotnet build TowerOps.sln`
  - `dotnet test TowerOps.sln --logger "console;verbosity=minimal"`

## Screenshots / Evidence
- Attach UI screenshots, logs, or request/response samples if applicable.

## Risk and Rollback
- Risk level: Low / Medium / High
- Rollback approach:
  - [ ] Revert PR
  - [ ] Feature flag disable
  - [ ] Hotfix required

## Production Checklist
- [ ] No secrets committed
- [ ] DB migrations reviewed
- [ ] Docs updated (`docs/Api-Doc.md`, relevant phase docs)
- [ ] Observability impact considered (logs/metrics/alerts)
