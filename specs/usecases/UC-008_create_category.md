# UC-008 — Crear categoría

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Crear categoría.  
**Linked requirements:** FR-009; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-014  
**Endpoint propuesto:** `POST /api/categories`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor proporciona nombre y descripción opcional.

## Main Success Scenario
1. El actor proporciona nombre y descripción opcional.
2. El sistema valida nombre y unicidad.
3. El sistema registra la categoría.
4. Devuelve 201 con ID y datos creados.

## Alternative and Exception Flows
- A1: 400: nombre vacío o demasiado largo.
- A2: 409: nombre duplicado ignorando mayúsculas/minúsculas.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Existe una nueva categoría.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
