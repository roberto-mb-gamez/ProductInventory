# UC-011 — Actualizar categoría

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Actualizar categoría.  
**Linked requirements:** FR-012; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-007, BR-014, BR-022  
**Endpoint propuesto:** `PUT /api/categories/{id}`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor indica ID, nombre y descripción opcional.

## Main Success Scenario
1. El actor indica ID, nombre y descripción opcional.
2. El sistema comprueba existencia, validez y unicidad del nuevo nombre.
3. El sistema actualiza la categoría sin cambiar asociaciones.
4. Devuelve 200 con categoría actualizada.

## Alternative and Exception Flows
- A1: 404: categoría inexistente.
- A2: 400: datos inválidos.
- A3: 409: nombre usado por otra categoría.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Categoría actualizada; productos asociados se conservan.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
