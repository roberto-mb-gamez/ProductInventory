# UC-002 — Consultar producto

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Consultar producto.  
**Linked requirements:** FR-002; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-007  
**Endpoint propuesto:** `GET /api/products/{id}`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor proporciona el ID del producto.

## Main Success Scenario
1. El actor proporciona el ID del producto.
2. El sistema localiza el producto y sus relaciones.
3. El sistema proyecta datos de Product, Category y Supplier.
4. Devuelve 200 con el detalle.

## Alternative and Exception Flows
- A1: 404: producto inexistente.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- No se modifican datos.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
