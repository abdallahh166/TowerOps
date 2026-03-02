# BC-05 Staging Rollout Verification

Date: 2026-03-02  
Owner: Backend / Platform / QA  
Scope: External staging runtime verification for BC-05 (`retention`, `legal hold`, `self-data export`)

## 1) Objective

Validate BC-05 behavior on the deployed staging environment (not local-equivalent only):
- API reachable on public staging domain
- Auth flow works
- `POST /api/data-exports/me` works for authenticated user
- `GET /api/data-exports/me/{requestId}` returns JSON file
- No 5xx/log critical errors during flow

Target:
- `https://towerops.runasp.net`

## 2) Execution Evidence (External Staging)

Execution time:
- UTC: `2026-03-02 00:08:02`
- Africa/Cairo: `2026-03-02 02:08:02`

Observed responses:

| Check | Request | Result |
|---|---|---|
| Root reachability | `GET /` | `403 Forbidden` |
| Health endpoint | `GET /health` | `404 Not Found` |
| Swagger endpoint | `GET /swagger/index.html` | `404 Not Found` |
| Auth route probe | `POST /api/auth/login` | IIS HTML `404 - File or directory not found` |

Conclusion:
- External staging API routing is not serving ASP.NET API endpoints correctly.
- BC-05 runtime smoke cannot proceed until deployment/routing is fixed.

## 3) Blocker Tracking

- GitHub issue: `#70`
- Link: `https://github.com/abdallahh166/TowerOps/issues/70`
- Severity: Release blocker for final staging GO

## 4) Required Remediation (One by One)

1. Verify published API artifact exists in the expected monsterasp site path.
2. Verify ASP.NET Core hosting configuration (`web.config`, module handler, process path).
3. Verify route base path: if hosted under subpath, update smoke URLs and docs accordingly.
4. Redeploy staging.
5. Re-run external smoke sequence below.

## 5) Re-run Smoke Sequence (After Fix)

```powershell
# 1) Health
Invoke-WebRequest https://towerops.runasp.net/health -Method GET

# 2) Login
$loginBody = @{ email = "admin@towerops.com"; password = "P@ssw0rd123!" } | ConvertTo-Json
$login = Invoke-RestMethod https://towerops.runasp.net/api/auth/login -Method POST -ContentType "application/json" -Body $loginBody
$token = $login.accessToken

# 3) Create my export request
$headers = @{ Authorization = "Bearer $token" }
$create = Invoke-RestMethod https://towerops.runasp.net/api/data-exports/me -Method POST -Headers $headers
$requestId = $create.requestId

# 4) Download export JSON
Invoke-WebRequest "https://towerops.runasp.net/api/data-exports/me/$requestId" -Method GET -Headers $headers -OutFile ".\\artifacts\\phase-2\\my-operational-data.json"
```

Acceptance:
- `health` returns `200`.
- login returns `200` with token.
- create export returns `200` with `requestId`.
- download returns `200` and JSON file.
- logs contain no critical errors for this flow.

## 6) Current Decision

- Decision: `NO-GO (External staging blocked)`
- Reason: Staging deployment/routing issue (`#70`) prevents API runtime verification.
