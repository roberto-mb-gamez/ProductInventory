# ADR-002 — CQRS manual, lecturas con EF Core y escrituras con Dapper
**Estado:** Aceptado

## Contexto
La prueba exige explícitamente Entity Framework para lecturas y Dapper para escrituras; recomienda CQRS cuando sea viable. El capítulo 12 del libro aborda CQRS y los compromisos relacionados con el código repetitivo.

## Decisión
- Implementar CQRS manualmente con tipos `Command` y `Query` explícitos y handlers enfocados, resueltos mediante inyección de dependencias. No utilizar MediatR, un mediador propio, un framework de handlers genéricos ni un bus de pipeline.
- Todas las consultas de aplicación usan EF Core 9, con proyecciones de solo lectura y `AsNoTracking` cuando corresponda. Los modelos de lectura pueden relacionar Product, Category y Supplier.
- Todas las escrituras de aplicación utilizan Dapper y SQL parametrizado. Los comandos pueden realizar lecturas transaccionales necesarias mediante la conexión Dapper/ADO.NET; no se utiliza EF Core para escrituras de negocio.
- Utilizar una sola base de datos SQL Server y un esquema lógico; sin réplicas de lectura ni proyecciones asíncronas.
- Definir contratos de Application específicos para cada intención, sin exigir un repositorio genérico.
- La creación del esquema y las migraciones pertenecen a Infrastructure; usar herramientas de EF para migraciones no se considera una escritura de negocio.

## Consecuencias
Handlers sencillos, rastreables y separación verificable entre lecturas y escrituras. Evitar consultas N+1 y propagar `CancellationToken` en operaciones de E/S.
