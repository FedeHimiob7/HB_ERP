# CLAUDE.md
te vas a comunicar conmigo simpre en español
This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project src/API/WebAPI/WebAPI.csproj

# Start RabbitMQ (required for messaging)
docker-compose up -d
```

Swagger UI is available at `/swagger` in Development mode. CORS is configured for `http://localhost:4200` by default (change `origenesPermitidos` in `appsettings.json`).

## Database Migrations

Each module has its own `DbContext` and migration history. Always specify both `--project` and `--startup-project`:

```bash
# Identity module
dotnet ef migrations add <Name> --project src/Modules/Identity/Identity.Infrastructure --startup-project src/API/WebAPI
dotnet ef database update            --project src/Modules/Identity/Identity.Infrastructure --startup-project src/API/WebAPI

# MasterData module
dotnet ef migrations add <Name> --project MasterData.Infrastructure --startup-project src/API/WebAPI
dotnet ef database update            --project MasterData.Infrastructure --startup-project src/API/WebAPI
```

## Architecture

This is a modular monolith following **DDD + Clean Architecture + CQRS**. There are two modules (`Identity`, `MasterData`) plus a shared kernel.

### Layer structure (same in every module)
```
*.Domain          → Entities, Value Objects, Domain Events (no external dependencies)
*.Application     → CQRS handlers (MediatR), FluentValidation validators, ErrorOr results
*.Infrastructure  → EF Core DbContext, repositories, EF interceptors, outbox publisher
```

### SharedKernel (`HB_ERP.SharedKernel/`)
All domain entities extend `AggregateRoot<TId>`. Key primitives:
- `AggregateRoot<TId>` — base class; call `Raise(domainEvent)` to queue domain events
- `DomainEvent` — base record for domain events
- `IAuditable` — shadow properties `CreatedAt`/`UpdatedAt` auto-set by `UpdateAuditableEntitiesInterceptor`
- `OutboxMessage` — used by both modules for reliable event publishing
- `PublishDomainEventsInterceptor` — fires domain events via MediatR on `SaveChangesAsync`
- Integration events live in `HB_ERP.SharedKernel/IntegrationEvents/` and are shared across modules

### Cross-module communication
Modules do **not** reference each other directly. Communication is async:
1. A domain event handler serializes an integration event into the `OutboxMessage` table (same transaction as the aggregate save).
2. The background service (`MasterDataOutboxPublisher` / `IdentityOutboxPublisher`) polls the outbox and publishes to RabbitMQ via MassTransit.
3. The consuming module has a MassTransit consumer that handles the integration event.

### CQRS conventions
- Commands/queries are records implementing `IRequest<ErrorOr<T>>`.
- Handlers return `ErrorOr<T>` — never throw for business errors.
- `ValidationBehavior<TRequest, TResponse>` (MediatR pipeline) runs FluentValidation before every handler.
- Controllers map `ErrorOr` results to HTTP responses using the `MatchFirst` / `Problem` pattern.

### Adding a new entity to an existing module
1. Create the aggregate in `*.Domain` extending `AggregateRoot<TId>`.
2. Add commands/queries + handlers in `*.Application`; add a FluentValidation validator in the same folder.
3. Add the DbSet and Fluent API configuration in `*.Infrastructure/Persistence`.
4. Run `dotnet ef migrations add` for the relevant module (see above).
5. Register any new services in the module's `DependencyInjection` extension method.

### Solution layout
```
src/
  API/WebAPI/                        ← Entry point; Program.cs wires all modules
  Modules/Identity/
    Identity.Domain/
    Identity.Application/
    Identity.Infrastructure/
    Identity.Integration/            ← MassTransit consumers for incoming events
    Identity.Shared/                 ← DTOs shared with API
  SharedKernel/Utils/
HB_ERP.SharedKernel/                 ← DDD primitives, interceptors, integration events
MasterData.Domain/
MasterData.Application/
MasterData.Infrastructure/
```

## Key packages
| Package | Purpose |
|---------|---------|
| MediatR 14 | CQRS dispatcher |
| ErrorOr 2 | Result type — use instead of exceptions for business errors |
| FluentValidation 12 | Command/query validation via MediatR pipeline |
| MassTransit 9 + RabbitMQ | Async integration events |
| EF Core 9 | ORM; two separate DbContexts (Identity schema, MasterData schema) |
| Ardalis.GuardClauses | Input guards in domain constructors |
| RT.Comb | Sequential GUIDs for PKs |
| Serilog | Structured logging; writes to SQL Server (`LogErrorHB_ERP` DB) |
