# TelecomPM API Layer

## Overview
The API layer exposes the domain/application capabilities over a RESTful ASP.NET Core Web API (net8.0). It follows Clean Architecture principles, acting solely as an orchestration layer that translates HTTP requests into MediatR commands/queries and returns consistent HTTP responses based on the `Result` pattern.

## Tech Stack
- ASP.NET Core Web API (controllers)
- MediatR integration via Application layer
- Serilog (console/file sinks)
- Swagger / OpenAPI (Swashbuckle)
- System.Text.Json with string enums

## Structure
```
TelecomPm.Api/
├── Contracts/        # Request DTOs & query parameters
├── Controllers/      # API endpoints (Visits, Sites, Materials, Reports)
├── Program.cs        # Host + DI setup
├── appsettings*.json # Environment-specific configuration
```

## Key Concepts
- **ApiControllerBase** centralizes `Result` → `IActionResult` translation (200/201/204/400/404).
- **Contracts** keep HTTP payloads decoupled from application commands.
- **CORS policy** driven by configuration (`Cors:AllowedOrigins`).
- **Global concerns**: Serilog logging, ProblemDetails, health checks, Swagger UI.

## Main Endpoints
- `POST /api/visits` → `CreateVisitCommand`
- `POST /api/visits/{id}/start|complete|submit`
- `POST /api/visits/{id}/approve|reject|request-correction`
- `POST /api/visits/{id}/photos|readings`
- `GET /api/visits/{id}` (detail), `/engineers/{engineerId}`, `/pending-reviews`, `/scheduled`
- `GET /api/sites/{id}`, `/office/{officeId}`, `/maintenance`
- `GET /api/materials/low-stock/{officeId}`
- `GET /api/reports/visits/{visitId}`

All endpoints call MediatR, so domain logic remains in the Application layer.

## Running Locally
```bash
dotnet run --project src/TelecomPm.Api
```
Swagger UI: `https://localhost:5001/swagger`

## Testing
The API layer relies on existing unit tests in Domain/Application/Infrastructure. Add integration tests in a future `TelecomPm.Api.Tests` project if needed.***

