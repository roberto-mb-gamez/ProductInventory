# Matriz de trazabilidad — ProductInventory v2.1

| Requisito | Caso de uso |
|---|---|
| FR-001 | UC-001 |
| FR-002 | UC-002 |
| FR-003 | UC-003 |
| FR-004 | UC-004 |
| FR-005 | UC-005 |
| FR-006 | UC-006 |
| FR-007 | UC-007 |
| FR-008 | UC-007 |
| FR-009 | UC-008 |
| FR-010 | UC-009 |
| FR-011 | UC-010 |
| FR-012 | UC-011 |
| FR-013 | UC-012 |
| FR-014 | UC-013 |
| FR-015 | UC-014 |
| FR-016 | UC-015 |
| FR-017 | UC-016 |
| FR-018 | UC-017 |

**Seguridad:** UC-007 es precondición transversal de UC-001 a UC-006 y UC-008 a UC-017.

**UC-006:** FR-006 exige registrar movimientos IN/OUT con persistencia atómica; BR-023 a BR-027 detallan la integridad del historial. UC-005 protege movimientos históricos.

La matriz siguiente enlaza la implementación y las pruebas de la etapa 5.4.

## Implementación verificada — etapa 5.4
Estado: 17/17 COMPLETED el 2026-11-12, pendiente de aprobación humana. Evidencia detallada local: docs/implementation/evidence/STAGE-5.4-use-cases.md; resultados TRX en test-results-5.4.

| UC | Requisito | Pruebas de integración / comprobación | Resultado |
|---|---|---|---|
| UC-001 | FR-001 | ProductApiTests.UC001*: creación, SKU duplicado, validación, referencias, stock cero | 201/400/404/409 |
| UC-002 | FR-002 | ProductApiTests.UC002*: detalle con Category/Supplier e inexistente | 200/404 |
| UC-003 | FR-003 | ProductApiTests.UC003*: filtros AND, paginación y límites | 200/400 |
| UC-004 | FR-004 | ProductApiTests.UC004*: relaciones, SKU, inexistente y conservación del stock | 200/400/404/409 |
| UC-005 | FR-005 | ProductApiTests.UC005*: borrado y protección del historial | 204/404/409 |
| UC-006 | FR-006, BR-023–027 | MovementApiTests.UC006*: IN/OUT, UTC, saldos, rollback, concurrencia; StockMovementPersistenceTests | 201/400/404/409, rollback ante 500 |
| UC-007 | FR-007/008 | Seguridad manual aprobada en 5.3; smoke de las 16 operaciones en Compose sin token | 16 respuestas 401; JWT/PKCE mantienen controles de 5.3 |
| UC-008 | FR-009 | CategoryApiTests.UC008*: creación, unicidad y validación | 201/400/409 |
| UC-009 | FR-010 | CategoryApiTests.UC009*: detalle e inexistente | 200/404 |
| UC-010 | FR-011 | CategoryApiTests.UC010*: orden, página, límites y página extrema | 200/400 |
| UC-011 | FR-012 | CategoryApiTests.UC011* y ApiContractTests.UC011*: actualización, unicidad y conservación | 200/400/404/409 |
| UC-012 | FR-013 | CategoryApiTests.UC012*: borrado y protección de productos | 204/404/409 |
| UC-013 | FR-014 | SupplierApiTests.UC013*: creación, nombres duplicados permitidos y email | 201/400 |
| UC-014 | FR-015 | SupplierApiTests.UC014*: detalle e inexistente | 200/404 |
| UC-015 | FR-016 | SupplierApiTests.UC015*: orden y paginación | 200/400 |
| UC-016 | FR-017 | SupplierApiTests.UC016*: actualización, email e inexistente | 200/400/404 |
| UC-017 | FR-018 | SupplierApiTests.UC017*: borrado y protección de productos | 204/404/409 |

Además, Domain.Tests valida invariantes y Application.Tests valida handlers/puertos. ApiContractTests verifica siete rutas y 16 operaciones OpenAPI, contrato 201 de movimientos y errores genéricos. Las pruebas HTTP de negocio usan identidad exclusiva del host de pruebas y SQL Server real: no prueban JWT y no contactan Keycloak. La validación de seguridad con tokens válidos e inválidos permanece manual por instrucción del responsable.
