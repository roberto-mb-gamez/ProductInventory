# ProductInventory — Codex repository instructions

## Task and authority
Implement the approved ProductInventory v2.2 technical assessment. Before editing code, read `constraints.md`, `specs/requirements_catalog.md`, `specs/entity_model.md`, the relevant `specs/usecases/UC-*.md`, and accepted `docs/adr/ADR-*.md`.
Authority: assessment > approved functional specs > accepted ADRs > architectural book guidance. If conflicting, stop and describe the conflict; do not silently alter approved scope. No extra endpoints or patterns without approval.

## Approved stack
- Target `net9.0`; ASP.NET Core 9 Minimal APIs; four source projects: Api, Application, Domain, Infrastructure.
- Manual CQRS; no MediatR or custom mediator. Queries use EF Core 9; business writes use Dapper and parameterized SQL. One SQL Server database.
- Keycloak OAuth2/OIDC and JWT bearer; validate issuer, audience, signature and expiration. Swagger Authorization Code + PKCE; all business routes protected.
- Docker Compose runs API, SQL Server and Keycloak; include reproducible schema and dev realm setup.
- xUnit + Moq for unit tests; WebApplicationFactory and real SQL Server for integration tests.

## Layer rules
- Domain: framework-independent entities and invariants; no EF, Dapper, HTTP, DI or SQL references.
- Application: explicit commands/queries, focused handlers, DTOs and ports. Reference Domain only.
- Infrastructure: implement Application ports; EF read projections and Dapper transactional writes.
- Api: thin endpoint mapping, JWT middleware, Swagger, DI composition. No SQL access in endpoints.
- No generic repository requirement, event sourcing, messaging, microservices, extra shared project or speculative abstractions.

## Inventory invariants
- Product stock starts at zero; only UC-006 changes it.
- UC-006 is `POST /api/products/{id}/movements`: immutable IN/OUT movement, strictly positive quantity, UTC timestamp and before/after stock.
- Update Product stock and insert StockMovement in one SQL transaction. Guard concurrent operations and reject insufficient stock without partial effects.
- Do not delete Product with movements, or Category/Supplier with products. No cascade deletion of movement history.
- Preserve all 17 approved use cases and their documented status codes/acceptance criteria.

## Assessment-specific code rules
- All code, identifiers and comments in English, except user-facing response messages.
- No blank lines between lines of authored code, except one blank line before each `return` where applicable.
- Self-explanatory methods; single responsibility; normally <=25 lines; avoid nested loops/conditionals beyond one level.
- One class or enum per file; no commented-out code; prefer parameter objects to long primitive parameter lists.
- Runtime settings come from environment variables; never commit real secrets or production credentials.

## Execution workflow
1. Select one UC and read its FR, BR and acceptance criteria.
2. Write a failing test first (red); implement minimal behavior (green); refactor.
3. Keep command writes in Dapper and query reads in EF Core; maintain dependency direction.
4. Add integration tests for relational joins, transactional rollback, concurrency and authentication as relevant.
5. Run relevant tests, build, Compose startup and Swagger checks; report actual outcomes only.
6. Update README, specs or ADRs when approved changes require it; do not change business behavior implicitly.
7. Request explicit approval for scope changes, destructive operations or deviations from ADRs.

## Runtime support note
.NET 9 is user-selected despite end of support on 2026-11-10. Do not upgrade without approval.

## ADR translations
Spanish translations of the eight accepted ADRs are available under `docs/adr/es/`. Treat `docs/adr/ADR-*.md` as the operational source of truth for Codex. Keep both language versions synchronized when an ADR changes. Spanish documentation does not authorize Spanish source code or comments.
