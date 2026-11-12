# ProductInventory v2.2 — Approved Architecture Package

This is a **specification and architecture package**, not an implemented application.

## Status
- Functional scope: approved 17 use cases (UC-006 aligned to mandatory inventory movement registration).
- Architecture and stack: approved; ADR-001 through ADR-008 accepted.
- Implementation: not started. No tests, builds or containers have been executed.

## Read in this order
1. `constraints.md` — assessment requirements and approved technology choices.
2. `specs/vision.md` and `specs/requirements_catalog.md` — product scope, FR, NFR and business rules.
3. `specs/entity_model.md` — four entities and relationships.
4. `specs/use_cases.puml` and `specs/usecases/` — 17 approved use cases.
5. `docs/adr/ADR-001` through `ADR-008` — rationale and architectural decisions.
6. `AGENTS.md` — Codex execution instructions.

## Stack
.NET 9 / ASP.NET Core Minimal APIs / Clean Architecture (four projects) / manual CQRS / EF Core 9 reads / Dapper writes / SQL Server / Keycloak OAuth2 JWT / Docker Compose / xUnit + Moq / WebApplicationFactory / Swagger.

## Key domain clarification
`StockMovement` is mandatory. UC-006 registers an immutable IN/OUT movement and updates stock transactionally; UC-005 blocks deletion of products with movement history.

## Next step
Use the specs and AGENTS.md as input for Codex, implementing one use case at a time with TDD. Document exact package patch versions and deployment settings at implementation time. The selected .NET 9 target is out of support and should be upgraded when local tooling allows.

## ADR en español
Los ocho ADR aprobados se conservan en inglés en `docs/adr/` y están traducidos al español en `docs/adr/es/`, manteniendo sus identificadores, decisiones y estado. Los archivos en inglés siguen siendo la referencia operativa para Codex; las traducciones sirven para revisión y consulta. Si una decisión cambia, actualizar ambas versiones.
