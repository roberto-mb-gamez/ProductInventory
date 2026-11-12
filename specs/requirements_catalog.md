# Requirements Catalog — ProductInventory v2.1

**Estado:** Baseline funcional aprobada (17 UC), con adecuación obligatoria de UC-006 a la prueba técnica. Decisiones complementarias del Product Owner documentadas.

## Functional Requirements

| ID | Requerimiento | UC |
|---|---|---|
| FR-001 | Registrar producto con SKU, nombre, descripción opcional, precio, categoría y proveedor principal. | UC-001 |
| FR-002 | Consultar un producto por ID con datos de categoría y proveedor. | UC-002 |
| FR-003 | Listar productos paginados, con filtros opcionales por categoría, proveedor y stock máximo. | UC-003 |
| FR-004 | Actualizar datos y relaciones de un producto, sin modificar existencias. | UC-004 |
| FR-005 | Eliminar un producto sin movimientos históricos; conservar la integridad del historial. | UC-005 |
| FR-006 | Registrar un movimiento de inventario de tipo IN u OUT, con cantidad entera positiva; actualizar existencias de forma atómica. | UC-006 |
| FR-007 | Permitir ejecutar operaciones protegidas al presentar un token de acceso OAuth2 válido. | UC-007 |
| FR-008 | Rechazar solicitudes anónimas o con token inválido a todas las operaciones. | UC-007 |
| FR-009 | Crear categoría. | UC-008 |
| FR-010 | Consultar categoría por ID. | UC-009 |
| FR-011 | Listar categorías. | UC-010 |
| FR-012 | Actualizar categoría. | UC-011 |
| FR-013 | Eliminar categoría sin productos asociados. | UC-012 |
| FR-014 | Crear proveedor. | UC-013 |
| FR-015 | Consultar proveedor por ID. | UC-014 |
| FR-016 | Listar proveedores. | UC-015 |
| FR-017 | Actualizar proveedor. | UC-016 |
| FR-018 | Eliminar proveedor sin productos asociados. | UC-017 |

## Non-Functional Requirements

| ID | Requerimiento | Verificación |
|---|---|---|
| NFR-001 | Errores de validación y negocio consistentes, sin revelar detalles internos. | Casos negativos de integración. |
| NFR-002 | Ajustes concurrentes de stock no generan pérdidas de actualización ni stock negativo. | Prueba de concurrencia con ajustes simultáneos. |
| NFR-003 | Operaciones y esquemas de entrada/salida documentados. | Revisión de contrato OpenAPI. |
| NFR-004 | Cada UC cuenta con pruebas de camino feliz y al menos una ruta negativa. | Matriz de cobertura de pruebas. |
| NFR-005 | La API valida autenticidad, vigencia, emisor y audiencia del token según el mecanismo elegido en etapa 4. | Pruebas de tokens válidos e inválidos. |
| NFR-006 | Todos los endpoints de negocio requieren autenticación. | Pruebas sin token sobre productos, categorías y proveedores. |
| NFR-007 | Las consultas de listados tienen paginación determinista y no devuelven más de 100 elementos por página. | Pruebas de parámetros y límites. |

## Business Rules

| ID | Regla |
|---|---|
| BR-001 | SKU obligatorio y único entre productos, ignorando mayúsculas/minúsculas. |
| BR-002 | Nombre de producto obligatorio, no compuesto solo por espacios. |
| BR-003 | Precio decimal mayor o igual a cero. |
| BR-004 | Existencias expresadas como entero mayor o igual a cero. |
| BR-005 | Todo producto nuevo inicia con cero existencias. |
| BR-006 | Un movimiento requiere tipo IN u OUT y cantidad entera mayor que cero; una salida no puede dejar stock negativo. |
| BR-007 | Operaciones sobre recursos inexistentes no alteran datos. |
| BR-008 | Actualizar datos de producto no modifica sus existencias; solo UC-006 puede cambiarlas. |
| BR-009 | SKU máximo 40 caracteres y nombre de producto máximo 120 caracteres. |
| BR-010 | Todas las operaciones exigen token de acceso válido. |
| BR-011 | Solicitudes sin autenticación válida no ejecutan operaciones ni modifican datos. |
| BR-012 | Cada producto referencia exactamente una categoría existente. |
| BR-013 | Cada producto referencia exactamente un proveedor principal existente. |
| BR-014 | Nombre de categoría obligatorio, único sin distinguir mayúsculas/minúsculas y máximo 100 caracteres. |
| BR-015 | No puede eliminarse una categoría con productos asociados. |
| BR-016 | Nombre de proveedor obligatorio y máximo 150 caracteres; email opcional, pero válido si se proporciona. |
| BR-017 | No puede eliminarse un proveedor con productos asociados. |
| BR-018 | Eliminar categorías o proveedores nunca elimina productos en cascada. |
| BR-019 | Una referencia inexistente a categoría o proveedor invalida la creación o actualización del producto. |
| BR-020 | Las consultas de listado usan página desde 1, tamaño entre 1 y 100; valores por defecto página 1 y tamaño 20. |
| BR-021 | Una categoría o proveedor puede existir sin productos asociados. |
| BR-022 | La actualización de SKU y de nombre de categoría respeta las reglas de unicidad, excluyendo el registro actualizado. |
| BR-023 | Cada movimiento registra producto, tipo, cantidad, instante UTC, stock anterior y posterior. |
| BR-024 | Registrar movimiento y modificar stock es una única operación atómica; ante fallo no se guarda ninguno. |
| BR-025 | Los movimientos son inmutables; no se editan ni eliminan en el MVP. |
| BR-026 | No se elimina un producto que tenga movimientos de inventario registrados. |
| BR-027 | Los movimientos concurrentes no pueden perder actualizaciones ni dejar stock negativo. |

## Restricciones técnicas
Las restricciones obligatorias C-001 a C-019 se documentan en [`../constraints.md`](../constraints.md). La arquitectura propuesta está pendiente de aprobación técnica. OAuth2 protege recursos mediante tokens emitidos externamente; no se implementa login propio.

## Códigos HTTP esperados (propuesta de contrato)
- `200` consulta/actualización/registro de movimiento correcto; `201` creación; `204` eliminación.
- `400` entrada inválida; `401` credenciales ausentes/inválidas; `404` recurso inexistente; `409` unicidad, asociación impeditiva o conflicto de concurrencia.
- `403` solo si se añaden permisos específicos en una etapa futura; no forma parte de la lógica de roles del MVP.
