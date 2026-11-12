# UC-013 — Crear proveedor

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Crear proveedor.  
**Linked requirements:** FR-014; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-016  
**Endpoint propuesto:** `POST /api/suppliers`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El actor proporciona nombre y correo opcional.

## Main Success Scenario
1. El actor proporciona nombre y correo opcional.
2. El sistema valida nombre y formato de correo si existe.
3. El sistema registra el proveedor.
4. Devuelve 201 con ID y datos creados.

## Alternative and Exception Flows
- A1: 400: nombre vacío, excesivamente largo o correo inválido.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Existe un nuevo proveedor.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
