# ProductInventory — Constraints and Architecture Baseline
Status: Approved — Architecture v2.2

## Authority order
1. The technical assessment (mandatory requirements).
2. The approved 17-use-case functional baseline in `specs/`, aligned to mandatory inventory movements in UC-006.
3. Accepted architecture decisions in `docs/adr/`.
4. *Clean Architecture with .NET* (Crouse and Smith, Packt 2026) as guidance, not an automatic mandate to copy tools or sample architecture.
5. Explicit Product Owner decisions for remaining functional ambiguities.

## Mandatory assessment constraints
- **C-001**: .NET 8 or newer; chosen target **.NET 9** (ADR-005).
- **C-002**: SQL Server relational database.
- **C-003**: At least Products, Categories and InventoryMovements tables; Suppliers also required by approved scope.
- **C-004**: Entity Framework Core for application reads; Dapper for application writes (ADR-002).
- **C-005**: REST CRUD for Products and Categories; approved scope also requires Supplier CRUD.
- **C-006**: Endpoint to register inventory IN/OUT movements; persist movement and stock atomically (ADR-003).
- **C-007**: Protect business endpoints using OAuth2 access tokens (ADR-006).
- **C-008**: Dockerize API and SQL Server; one `docker-compose.yml` starts the environment (ADR-007).
- **C-009**: Unit tests for critical components and TDD (ADR-004).
- **C-010**: Swagger/OpenAPI documentation of endpoints (ADR-005).
- **C-011**: README includes setup, execution, tests, deployment and local debugging.
- **C-012**: Publish all necessary source and deployment artifacts in a public repository.
- **C-013**: OOP, SOLID, Clean Code, design patterns and CQRS where feasible (adopted manually).
- **C-014**: Source code and comments 100% English, except user-facing messages.
- **C-015**: No blank lines between code lines, except one blank line before each return.
- **C-016**: Self-explanatory methods, single responsibility, normally <=25 lines, avoid nested loops/conditionals deeper than one level.
- **C-017**: Runtime configuration read from environment variables.
- **C-018**: One class or enum per file; no commented-out code.
- **C-019**: Prefer passing objects rather than long lists of individual properties.

## Approved architecture and technology choices
- **A-001**: Single deployable API; four source projects: Api, Application, Domain, Infrastructure (ADR-001).
- **A-002**: Manual CQRS; EF Core 9 for queries, Dapper with parameterized stored procedure calls for commands, one SQL Server DB, no MediatR (ADR-002; writer procedure migration explicitly authorized on 2026-11-12).
- **A-003**: Domain independent of infrastructure; intent-specific persistence ports; no mandatory generic repository (ADR-001/002).
- **A-004**: Product, Category, Supplier, StockMovement; transactional, concurrency-safe movements (ADR-003).
- **A-005**: ASP.NET Core 9 Minimal APIs, `net9.0` (ADR-005).
- **A-006**: Keycloak OAuth2/OIDC, JWT bearer validation; Swagger Authorization Code + PKCE (ADR-006).
- **A-007**: Compose starts API, SQL Server and Keycloak (ADR-007).
- **A-008**: xUnit, Moq, native xUnit assertions, WebApplicationFactory and SQL Server-backed integration tests (ADR-004).
- **A-009**: Apply assessment code standards and publish reproducible source (ADR-008).

## Explicit exclusions
No microservices, distributed CQRS, message broker, Event Sourcing, generic repository framework, custom mediator, custom login, user-management API, multi-warehouse inventory or movement edit/delete endpoints.

## Operational cautions
- .NET 9 support ended 2026-11-10; it remains user-selected for this exercise. Migration to .NET 10 LTS requires approval.
- A Docker development realm and local passwords are not production-grade identity configuration.
- No code, container startup or test execution has yet been performed.
