# ProductInventory — Especificaciones funcionales v2

**Estado:** Draft para revisión funcional. **Propósito:** base de análisis para una prueba técnica Senior; no contiene código ni decisiones de arquitectura.

## Alcance aprobado
- Gestión CRUD de productos, categorías y proveedores.
- Ajuste de existencias de productos.
- Acceso a todas las operaciones mediante tokens de acceso OAuth2 válidos.
- Consultas de productos con categoría y proveedor, filtros y paginación.

## Artefactos
1. `vision.md`: visión, actores, alcance y exclusiones.
2. `requirements_catalog.md`: FR, NFR, reglas de negocio y restricciones conocidas sin diseño técnico.
3. `entity_model.md`: modelo conceptual, cardinalidades y atributos.
4. `use_cases.puml`: diagrama de casos de uso PlantUML.
5. `usecases/UC-001...UC-017`: especificaciones verificables con flujos y aceptación.
6. `traceability.md`: matriz FR → UC y cobertura.

## Convenciones
- **FR-###**: requerimiento funcional; **NFR-###**: no funcional; **BR-###**: regla de negocio; **UC-###**: caso de uso.
- `401` significa autenticación ausente o inválida; `403` se reserva para una eventual autorización adicional (fuera del MVP).
- Los endpoints indicados son **contratos funcionales propuestos**, no decisiones de framework.
- Las restricciones del enunciado (REST, SQL Server, Docker, OAuth2, SOLID) se registran sin seleccionar implementación; su formalización queda para la etapa 4.
- El conjunto es intencionalmente pequeño: sin órdenes, compras, ventas, múltiples almacenes, movimientos históricos ni administración de usuarios.


## v2.1 reconciliation
UC-006 is now `UC-006_register_inventory_movement.md`. The technical assessment requires an InventoryMovements table. Product deletion is blocked when movement history exists. See `../constraints.md` and the repository README.
