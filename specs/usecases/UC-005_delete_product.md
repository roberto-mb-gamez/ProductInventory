# UC-005 — Eliminar producto

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Eliminar producto.  
**Linked requirements:** FR-005; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-007  
**Endpoint propuesto:** `DELETE /api/products/{id}`

## Preconditions
- El producto no tiene movimientos de inventario asociados.
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor indica el ID de un producto.

## Main Success Scenario
1. El actor indica el ID de un producto.
2. El sistema verifica su existencia.
3. El sistema elimina únicamente el producto.
4. Devuelve 204 sin cuerpo.

## Alternative and Exception Flows
- A-HISTORY: Si el producto tiene movimientos registrados, devolver `409` sin eliminar producto ni historial.
- A1: 404: producto inexistente.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- El producto ya no existe; categoría y proveedor permanecen.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-HISTORY:** Eliminar un producto con movimientos devuelve `409` y preserva el historial.
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
