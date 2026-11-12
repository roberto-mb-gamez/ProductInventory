# ADR-001 — Lightweight Clean Architecture
Status: Accepted

## Context
ProductInventory is a small RESTful inventory technical assessment. The book *Clean Architecture with .NET* (Crouse and Smith, Packt 2026), especially chapters 2–5, motivates inward dependencies and also warns about excess ceremony in CRUD-heavy applications.

## Decision
Build one deployable monolith with four source projects: `ProductInventory.Domain`, `ProductInventory.Application`, `ProductInventory.Infrastructure`, `ProductInventory.Api`.
- Domain: entities, invariants, domain rules; no ASP.NET, EF Core, Dapper or SQL Server dependencies.
- Application: commands, queries, handlers, DTOs and persistence ports; references Domain.
- Infrastructure: implementations of Application ports, EF Core read model, Dapper write operations, SQL Server configuration; references Application and Domain.
- Api: HTTP endpoints, JWT bearer authentication, OpenAPI/Swagger, dependency-injection composition root; references Application and Infrastructure.
- Project references must never create inward-to-outward dependencies. API must not bypass Application to call persistence directly.

## Alternatives rejected
One-project CRUD API (less separation); full enterprise architecture (excess complexity).

## Consequences
Explicit boundaries, easy unit testing, moderate project count. No microservices, message broker, generic repository requirement, event sourcing, or extra Common project.
