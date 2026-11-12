# UC-007 — Acceder a API protegida

**Status:** Draft  
**Actor:** Administrador de Inventario (cliente autenticado)  
**Goal:** Acceder a API protegida.  
**Linked requirements:** FR-007, FR-008; NFR-001, NFR-004, NFR-006  
**Business rules:** BR-010, BR-011  
**Endpoint propuesto:** `N/A Todos los endpoints de negocio`

## Preconditions
- La API está disponible.
- El cliente presenta un token de acceso OAuth2 válido (UC-007); la obtención del token ocurre fuera del sistema.

## Trigger
- El cliente obtiene externamente un token de acceso OAuth2.

## Main Success Scenario
1. El cliente obtiene externamente un token de acceso OAuth2.
2. El cliente envía el token al solicitar una operación de negocio.
3. La API verifica autenticidad, vigencia y destinatario del token.
4. Si es válido, continúa al caso de uso solicitado.

## Alternative and Exception Flows
- A1: 401: token ausente, expirado, manipulado o no aceptado.
- A2: 403: reservado para futura autorización granular; no exigido en MVP.
- S1: Sin token o con token inválido: 401, no se ejecuta la operación (UC-007).

## Postconditions — Success
- Ninguna operación se ejecuta con token inválido.

## Postconditions — Failure
- No hay modificaciones parciales; si la operación falla, se conserva el estado previo.

## Acceptance Criteria
- **AC-01:** Dadas las precondiciones, cuando se ejecuta el flujo principal con datos válidos, se obtiene el resultado indicado y se cumple la postcondición.
- **AC-02:** Cada alternativa documentada devuelve su resultado esperado sin efectos no autorizados.
- **AC-03:** Sin token o con token inválido, se devuelve 401 sin ejecutar el caso de uso.
