# ADR-004 — TDD and layered testing
Status: Accepted

## Decision
- Unit tests: xUnit and Moq; use native xUnit assertions to avoid unnecessary assertion dependencies.
- `ProductInventory.Domain.Tests`: domain invariants.
- `ProductInventory.Application.Tests`: handlers, validation, success/failure paths using mocks/fakes.
- `ProductInventory.IntegrationTests`: SQL Server-backed EF Core projections, Dapper writes, inventory atomicity/concurrency, API status codes, JWT security; ASP.NET Core `WebApplicationFactory` where useful.
- Apply red → green → refactor per use case. Prioritize duplicate SKU, missing relations, insufficient stock, rollback, concurrency and 401/403 behavior.
- Collect coverage reports; no invented numeric threshold. Never report tests as passing unless executed.

## Consequences
Integration tests require a reproducible SQL Server environment, either Compose or disposable containers. Authentication tests must include negative paths.
