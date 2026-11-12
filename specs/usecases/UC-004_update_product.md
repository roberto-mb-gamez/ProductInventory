# UC-004 — Actualizar producto

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Actualizar producto.  
**Linked requirements:** FR-004; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-001, BR-002, BR-003, BR-007, BR-008, BR-009, BR-012, BR-013, BR-019, BR-022  
**Endpoint propuesto:** `PUT /api/products/{id}`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor indica ID y datos completos editables del producto.

## Main Success Scenario
1. El actor indica ID y datos completos editables del producto.
2. El sistema comprueba existencia, validez, unicidad y referencias.
3. El sistema reemplaza campos editables sin tocar StockQuantity.
4. Devuelve 200 con producto actualizado.

## Alternative and Exception Flows
- A1: 404: producto o referencia inexistente.
- A2: 400: entrada inválida.
- A3: 409: SKU en uso por otro producto.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Los campos editables quedan actualizados; stock permanece igual.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
