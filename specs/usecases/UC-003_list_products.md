# UC-003 — Listar productos con filtros

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Listar productos con filtros.  
**Linked requirements:** FR-003; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-020  
**Endpoint propuesto:** `GET /api/products`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor solicita una página y filtros opcionales `categoryId`, `supplierId`, `maxStock`.

## Main Success Scenario
1. El actor solicita una página y filtros opcionales `categoryId`, `supplierId`, `maxStock`.
2. El sistema valida paginación y filtros; aplica condiciones combinadas.
3. El sistema ordena de forma determinista y proyecta categoría y proveedor.
4. Devuelve 200 con elementos, página, tamaño y total de resultados.

## Alternative and Exception Flows
- A1: 400: página/tamaño fuera de rango o maxStock negativo.
- A2: 200: sin coincidencias, lista vacía.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- No se modifican datos.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
