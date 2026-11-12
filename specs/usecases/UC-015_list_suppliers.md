# UC-015 — Listar proveedores

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Listar proveedores.  
**Linked requirements:** FR-016; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-021  
**Endpoint propuesto:** `GET /api/suppliers`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor solicita proveedores.

## Main Success Scenario
1. El actor solicita proveedores.
2. El sistema recupera el catálogo ordenado por nombre e ID.
3. El sistema prepara los datos de proveedores.
4. Devuelve 200 con lista, que puede estar vacía.

## Alternative and Exception Flows
- A1: 200: catálogo vacío.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- No se modifican datos.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
