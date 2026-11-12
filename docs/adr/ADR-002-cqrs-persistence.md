# ADR-002 — Manual CQRS, EF Core reads and Dapper writes
Status: Accepted

## Context
The assessment explicitly requires Entity Framework for reads and Dapper for writes; CQRS is preferred if feasible. The book discusses CQRS and boilerplate tradeoffs in chapter 12.

## Decision
- Implement CQRS manually: explicit `Command`/`Query` types and focused handlers, resolved through DI. No MediatR, homegrown mediator, generic handler framework or pipeline bus.
- All application query handlers use EF Core 9 with read-only projections (`AsNoTracking` when appropriate). Read models may join Product, Category and Supplier.
- All application write operations use Dapper with parameterized SQL Server stored procedure calls (`CommandType.StoredProcedure`). Procedures are specific to each create/update/delete operation and to inventory movement registration. Do not use EF Core to perform business writes.
- Readers retain EF Core projections, filtering and pagination. Application ports and Domain remain independent of procedure names, SQL parameters and deployment details.
- Version procedure definitions as separate `CREATE OR ALTER PROCEDURE` scripts under Infrastructure `Persistence/Schema/`. SchemaInitializer applies embedded scripts in ordinal filename order in one transaction, retaining the schema application lock through the deployment. Existing data is preserved; initialization installs or updates procedures on both existing and new databases. This is an idempotent baseline/procedure deployment, not a general table-migration framework.
- CRUD procedures return the existing DTO columns; deletion returns a scalar boolean rather than depending on affected-row counts with `SET NOCOUNT ON`. Inventory registration returns a movement only after commit and an explicit outcome output parameter. The inventory procedure owns the transaction; the writer does not wrap it in a second transaction. SQL errors retain the existing Application exception/status mappings.
- Use one SQL Server database and one logical schema; no read replica or asynchronous projections.
- Define intent-specific Application ports, not a mandatory generic repository.
- Schema creation/migrations are an infrastructure concern; using EF tooling for migrations is not an application write operation.

## Consequences
Simple traceable handlers and enforceable read/write separation. Avoid N+1 reads and use cancellation tokens for I/O.

## Approved amendment — 2026-11-12
The project owner authorized replacing writer inline SQL with stored procedures to support future reuse and tuning. This keeps the assessment requirement of EF reads and Dapper writes. Procedure contracts and scripts must be deployed alongside the corresponding application version; execution permissions can be granted separately from table writes. Initialization requires DDL permissions; a pre-provisioned runtime needs SELECT for EF reads and EXECUTE on the required procedures. No performance improvement is claimed without measurement. Regression tests must cover upgrades without data loss, direct procedure use, transaction rollback, cancellation and concurrency.
