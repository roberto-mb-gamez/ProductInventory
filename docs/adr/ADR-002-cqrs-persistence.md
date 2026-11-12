# ADR-002 — Manual CQRS, EF Core reads and Dapper writes
Status: Accepted

## Context
The assessment explicitly requires Entity Framework for reads and Dapper for writes; CQRS is preferred if feasible. The book discusses CQRS and boilerplate tradeoffs in chapter 12.

## Decision
- Implement CQRS manually: explicit `Command`/`Query` types and focused handlers, resolved through DI. No MediatR, homegrown mediator, generic handler framework or pipeline bus.
- All application query handlers use EF Core 9 with read-only projections (`AsNoTracking` when appropriate). Read models may join Product, Category and Supplier.
- All application write operations use Dapper with parameterized SQL. Commands may perform necessary transactional reads through the Dapper/ADO.NET write connection; do not use EF Core to perform business writes.
- Use one SQL Server database and one logical schema; no read replica or asynchronous projections.
- Define intent-specific Application ports, not a mandatory generic repository.
- Schema creation/migrations are an infrastructure concern; using EF tooling for migrations is not an application write operation.

## Consequences
Simple traceable handlers and enforceable read/write separation. Avoid N+1 reads and use cancellation tokens for I/O.
