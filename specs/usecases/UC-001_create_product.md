# UC-001 — Registrar producto

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Registrar producto.  
**Linked requirements:** FR-001; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-001, BR-002, BR-003, BR-005, BR-009, BR-012, BR-013, BR-019  
**Endpoint propuesto:** `POST /api/products`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor proporciona SKU, nombre, descripción opcional, precio, CategoryId y SupplierId.

## Main Success Scenario
1. El actor proporciona SKU, nombre, descripción opcional, precio, CategoryId y SupplierId.
2. El sistema valida campos, unicidad de SKU y existencia de ambas referencias.
3. El sistema crea el producto con existencias iniciales en cero.
4. Devuelve 201 con el producto creado y su identificador.

## Alternative and Exception Flows
- A1: 400: datos inválidos o referencias ausentes.
- A2: 404: categoría o proveedor referenciado inexistente.
- A3: 409: SKU duplicado.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Se registra exactamente un producto con stock 0 y referencias válidas.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
