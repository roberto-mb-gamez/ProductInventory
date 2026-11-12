# ADR-004 — TDD y pruebas por capas
**Estado:** Aceptado

## Decisión
- Pruebas unitarias con xUnit y Moq; utilizar aserciones nativas de xUnit para evitar dependencias adicionales innecesarias.
- `ProductInventory.Domain.Tests`: invariantes del dominio.
- `ProductInventory.Application.Tests`: handlers, validación y escenarios de éxito/error con mocks o fakes.
- `ProductInventory.IntegrationTests`: proyecciones EF Core sobre SQL Server, escrituras Dapper, atomicidad y concurrencia de inventario, códigos HTTP y seguridad JWT; utilizar `WebApplicationFactory` cuando aporte valor.
- Seguir el ciclo rojo → verde → refactorización para cada caso de uso. Priorizar SKU duplicado, relaciones inexistentes, stock insuficiente, rollback, concurrencia y respuestas 401/403.
- Recopilar reportes de cobertura sin inventar un porcentaje obligatorio. Nunca informar que las pruebas pasan sin ejecutarlas.

## Consecuencias
Las pruebas de integración necesitan SQL Server reproducible, mediante Compose o contenedores desechables. Las pruebas de autenticación deben incluir escenarios negativos.
