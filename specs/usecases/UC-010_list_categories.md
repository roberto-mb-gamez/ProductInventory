# UC-010 — Listar categorías

**Status:** Draft
**Actor:** Administrador de Inventario (cliente autenticado)
**Goal:** Listar categorías.
**Linked requirements:** FR-011; NFR-001, NFR-004, NFR-006
**Business rules:** BR-020, BR-021
**Endpoint propuesto:** `GET /api/categories`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor solicita categorías con page y pageSize opcionales.

## Main Success Scenario
1. El actor solicita categorías con page y pageSize opcionales.
2. El sistema valida page >= 1 y pageSize entre 1 y 100 (predeterminados 1 y 20) y recupera la página ordenada por nombre e ID.
3. El sistema prepara los datos de las categorías.
4. Devuelve 200 con items (puede estar vacío), page, pageSize y totalCount.

## Alternative and Exception Flows
- A1: 200: catálogo vacío, items vacío y totalCount 0.
- A2: 400: parámetros de paginación fuera de rango.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- No se modifican datos.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
