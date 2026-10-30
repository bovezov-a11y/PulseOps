# PulseOps

> Service monitoring and health check API

PulseOps keeps an eye on websites and web services. An organization registers the URLs it cares about, and a
background worker checks each of them over HTTP, recording the status code, response time and any error.
Every user belongs to an organization and only sees that organization's services (multi-tenant).

## Tech stack

| Layer          | Technology |
|----------------|------------|
| API            | .NET 8, ASP.NET Core Web API |
| Architecture   | Clean Architecture: Domain, Application, Infrastructure, Presentation |
| Database       | PostgreSQL (TimescaleDB image), Entity Framework Core with migrations |
| Security       | JWT bearer authentication, BCrypt password hashing |
| Documentation  | Swagger / OpenAPI (Swashbuckle) |
| Infrastructure | Docker Compose |

## Features

- **Sign-up and login:** registering creates the user and their organization and returns a JWT. Passwords are
  stored as BCrypt hashes.
- **Multi-tenant access:** the token carries an `organizationId` claim, and every query is filtered by it.
- **Monitored services:** add a service by name and URL, and list your services with their latest check.
- **Background health checks:** a hosted worker checks every active service every 15 seconds and stores a health
  log entry with the status code, response time in milliseconds and error message.

## Project structure

```
src/
  Core/
    PulseOps.Domain/           Entities (Organization, User, MonitoredService, ServiceHealthLog…) and enums
    PulseOps.Application/      DTOs and interfaces, such as IJwtTokenGenerator
  Infrastructure/
    PulseOps.Infrastructure/   EF Core DbContext and migrations, JWT generator, monitoring worker
  Presentation/
    PulseOps.Api/              Controllers, Swagger, startup configuration
```

## API

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/Auth/register` | Create a user and their organization; returns a JWT |
| `POST` | `/api/Auth/login` | Log in; returns a JWT |
| `POST` | `/api/Services` | Register a service to monitor (requires a JWT) |
| `GET`  | `/api/Services` | Your organization's services with their latest check (requires a JWT) |

Example requests are in [`PulseOps.Api.http`](src/Presentation/PulseOps.Api/PulseOps.Api.http).

## Running locally

Requirements: .NET 8 SDK and Docker.

```bash
# 1. Start PostgreSQL (port 5434)
docker compose up -d pulseops-db

# 2. Create the database schema
dotnet tool install --global dotnet-ef   # once
dotnet ef database update --project src/Infrastructure/PulseOps.Infrastructure --startup-project src/Presentation/PulseOps.Api

# 3. Run the API
dotnet run --project src/Presentation/PulseOps.Api --launch-profile http
```

Swagger UI: http://localhost:5187/swagger. Use **Authorize** with `Bearer <token>` to call the protected endpoints.

## Configuration

Settings come from `appsettings.json` and can be overridden with environment variables. Secrets are never committed.

| Variable | Purpose |
|----------|---------|
| `JwtSettings__Secret` | Key that signs tokens, at least 32 bytes (`openssl rand -base64 32`). **Required outside Development.** In Development a random key is generated on each start, so tokens stop working after a restart. |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection. Defaults to the Docker Compose database on port 5434. |
| `JwtSettings__ExpiryMinutes` | Token lifetime, 120 minutes by default. |

## Roadmap

- Respect each service's own check interval
- Alert rules (slow responses, consecutive failures) with email notifications; the `AlertRule` entity is in place
- Refresh tokens
- Uptime and latency history endpoints backed by TimescaleDB
- Tests and a CI pipeline
