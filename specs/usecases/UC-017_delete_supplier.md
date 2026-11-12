# UC-017 — Eliminar proveedor

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Eliminar proveedor.  
**Linked requirements:** FR-018; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-007, BR-017, BR-018  
**Endpoint propuesto:** `DELETE /api/suppliers/{id}`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor indica ID de proveedor.

## Main Success Scenario
1. El actor indica ID de proveedor.
2. El sistema comprueba existencia y ausencia de productos asociados.
3. El sistema elimina el proveedor.
4. Devuelve 204 sin cuerpo.

## Alternative and Exception Flows
- A1: 404: proveedor inexistente.
- A2: 409: proveedor con productos asociados.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Proveedor eliminado; ningún producto se elimina.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
