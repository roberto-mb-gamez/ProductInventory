# ADR-002 — CQRS manual, lecturas con EF Core y escrituras con Dapper
**Estado:** Aceptado

## Contexto
La prueba exige explícitamente Entity Framework para lecturas y Dapper para escrituras; recomienda CQRS cuando sea viable. El capítulo 12 del libro aborda CQRS y los compromisos relacionados con el código repetitivo.

## Decisión
- Implementar CQRS manualmente con tipos `Command` y `Query` explícitos y handlers enfocados, resueltos mediante inyección de dependencias. No utilizar MediatR, un mediador propio, un framework de handlers genéricos ni un bus de pipeline.
- Todas las consultas de aplicación usan EF Core 9, con proyecciones de solo lectura y `AsNoTracking` cuando corresponda. Los modelos de lectura pueden relacionar Product, Category y Supplier.
- Todas las escrituras de aplicación utilizan Dapper con llamadas parametrizadas a procedimientos almacenados de SQL Server (`CommandType.StoredProcedure`). Los procedimientos son específicos para cada creación/actualización/borrado y para el registro de movimientos. No se utiliza EF Core para escrituras de negocio.
- Los readers conservan las proyecciones, filtros y paginación de EF Core. Los puertos de Application y Domain permanecen independientes de nombres de procedimientos, parámetros SQL y detalles de despliegue.
- Versionar las definiciones como scripts separados con `CREATE OR ALTER PROCEDURE` en `Persistence/Schema/` de Infrastructure. SchemaInitializer aplica los scripts embebidos en orden ordinal de nombre dentro de una transacción, conservando el bloqueo de aplicación del esquema durante el despliegue. Se preservan los datos existentes; la inicialización instala o actualiza procedimientos tanto en bases existentes como nuevas. Es un despliegue idempotente de baseline/procedimientos, no un framework general de migraciones de tablas.
- Los procedimientos CRUD devuelven las columnas de los DTO actuales; el borrado devuelve un booleano escalar en lugar de depender del conteo de filas con `SET NOCOUNT ON`. El registro de inventario devuelve un movimiento solo después del commit y un parámetro de salida con el resultado explícito. El procedimiento de inventario controla la transacción; el writer no lo envuelve en una segunda transacción. Los errores SQL conservan el mapeo actual a excepciones/estados de Application.
- Utilizar una sola base de datos SQL Server y un esquema lógico; sin réplicas de lectura ni proyecciones asíncronas.
- Definir contratos de Application específicos para cada intención, sin exigir un repositorio genérico.
- La creación del esquema y las migraciones pertenecen a Infrastructure; usar herramientas de EF para migraciones no se considera una escritura de negocio.

## Consecuencias
Handlers sencillos, rastreables y separación verificable entre lecturas y escrituras. Evitar consultas N+1 y propagar `CancellationToken` en operaciones de E/S.

## Ajuste aprobado — 2026-11-12
El responsable autorizó reemplazar el SQL embebido de los writers por procedimientos almacenados para facilitar reutilización y ajustes futuros. Se conserva el requisito de lecturas EF y escrituras Dapper. Los contratos y scripts de procedimientos se despliegan junto con la versión correspondiente de la aplicación; los permisos de ejecución pueden otorgarse por separado de los de escritura en tablas. La inicialización requiere permisos DDL; un runtime con base preparada necesita SELECT para las lecturas EF y EXECUTE sobre los procedimientos requeridos. No se afirma una mejora de rendimiento sin medición. Las pruebas de regresión deben cubrir actualización sin pérdida de datos, uso directo de procedimientos, rollback, cancelación y concurrencia.
