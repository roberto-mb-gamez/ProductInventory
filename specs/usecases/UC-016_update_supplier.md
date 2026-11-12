# UC-016 — Actualizar proveedor

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Actualizar proveedor.  
**Linked requirements:** FR-017; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-007, BR-016  
**Endpoint propuesto:** `PUT /api/suppliers/{id}`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor indica ID, nombre y correo opcional.

## Main Success Scenario
1. El actor indica ID, nombre y correo opcional.
2. El sistema comprueba existencia y validez de los datos.
3. El sistema actualiza el proveedor sin alterar productos.
4. Devuelve 200 con proveedor actualizado.

## Alternative and Exception Flows
- A1: 404: proveedor inexistente.
- A2: 400: datos inválidos.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Proveedor actualizado; relaciones con productos se conservan.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
