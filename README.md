# TelecomPM

[![.NET CI](https://github.com/boda166/telecomPm/actions/workflows/dotnet.yml/badge.svg)](https://github.com/boda166/telecomPm/actions/workflows/dotnet.yml)
![Platform](https://img.shields.io/badge/platform-.NET%208%20API-blue)
![Tests](https://img.shields.io/badge/tests-xUnit-success)

TelecomPM is a field-operations platform for telecom maintenance subcontractors.  
It helps operations teams plan visits, execute work orders, track evidence, manage materials, and provide controlled client visibility.

## Why TelecomPM

- Standardized visit and work-order lifecycles
- GPS check-in/out and evidence-driven execution
- SLA monitoring, KPI dashboards, and operational reporting
- Excel import/export pipelines for real field templates
- Client portal with scoped, read-only data access
- Arabic/English API localization support (`ar-EG`, `en-US`)

## Core Modules

- Auth, Roles, Permissions
- Sites, Offices, Users
- Visits (checklists, photos, readings, issues)
- Work Orders and Escalations
- Materials and Stock
- Analytics, KPIs, Reports
- Offline Sync
- Dynamic System Settings

Full API surface: `docs/Api-Doc.md`

## Tech Stack

- ASP.NET Core Web API (`net8.0`)
- Clean Architecture (Domain / Application / Infrastructure / API)
- EF Core + SQL Server
- MediatR + FluentValidation
- Serilog
- ClosedXML (Excel)

## Quick Start

### 1) Prerequisites

- .NET SDK 9+ installed
- SQL Server available

### 2) Configure

- Update DB connection in `src/TelecomPm.Api/appsettings.json`
- Set JWT secret:

```powershell
$env:JWT_SECRET = "replace-with-strong-secret"
```

`JWT_SECRET` is required in Production.

### 3) Run

```bash
dotnet restore
dotnet build TelecomPM.sln
dotnet run --project src/TelecomPm.Api
```

Swagger is enabled in Development.

### 4) Test

```bash
dotnet test TelecomPM.sln --logger "console;verbosity=minimal"
```

## Documentation

- Documentation index (source-of-truth map): `docs/Documentation-Index.md`
- API Docs: `docs/Api-Doc.md`
- Operational Workflow: `docs/Operational-Workflow.md`
- Domain Notes: `docs/Domain-Doc.md`
- Application Notes: `docs/Application-Doc.md`
- Excel Gap Audit: `docs/Excel-Domain-Gap-Report.md`
- Remaining documentation gaps: `docs/Documentation-Gap-Report.md`

## Repository Structure

- `src/TelecomPM.Domain`
- `src/TelecomPm.Application`
- `src/TelecomPm.Infrastructure`
- `src/TelecomPm.Api`
- `tests/*`

## Contributing

- Keep business logic in Domain/Application layers.
- Keep migrations additive.
- Update docs when endpoints or behaviors change.
