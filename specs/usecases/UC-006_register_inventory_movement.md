# UC-006 — Registrar movimiento de inventario

**Status:** Approved — mandatory alignment with technical assessment
**Actor:** Administrador de Inventario (cliente autenticado)
**Goal:** Registrar una entrada o salida de producto y mantener sus existencias consistentes.
**Linked requirements:** FR-006; NFR-001, NFR-002, NFR-004, NFR-006
**Business rules:** BR-004, BR-005, BR-006, BR-007, BR-008, BR-023, BR-024, BR-025, BR-027
**Endpoint propuesto:** `POST /api/products/{id}/movements`

## Preconditions
- El cliente presenta token de acceso OAuth2 válido (UC-007).
- El producto existe.

## Trigger
- El actor envía el tipo `IN` o `OUT` y una cantidad entera positiva.

## Main Success Scenario
1. El actor identifica el producto e indica tipo y cantidad.
2. El sistema valida que el producto exista, que el tipo sea admitido y que la cantidad sea positiva.
3. El sistema determina las existencias anteriores y calcula las posteriores (`IN`: suma; `OUT`: resta).
4. El sistema rechaza salidas que generarían existencias negativas.
5. El sistema registra el movimiento y actualiza las existencias como una sola operación atómica, segura ante concurrencia.
6. El sistema devuelve `201 Created` con el identificador del movimiento, tipo, cantidad y existencias anterior/posterior.

## Alternative and Exception Flows
- A1: Tipo desconocido, cantidad cero/no positiva o inválida: `400`; sin cambios.
- A2: Producto inexistente: `404`; sin cambios.
- A3: Salida superior al stock disponible: `409`; sin cambios.
- A4: Conflicto de concurrencia no resuelto: `409`; sin cambios.
- S1: Token ausente, inválido o expirado: `401`; sin cambios.
- E1: Fallo de persistencia: se revierte la operación completa; no queda movimiento sin actualización ni actualización sin movimiento.

## Postconditions — Success
- Existe exactamente un movimiento persistido para la operación exitosa.
- `StockAfter = StockBefore + Quantity` para `IN`.
- `StockAfter = StockBefore - Quantity` para `OUT`.
- El stock actual coincide con `StockAfter` y nunca es negativo.

## Postconditions — Failure
- No se registra movimiento ni se modifica stock.

## Acceptance Criteria
- **AC-01:** Una entrada válida devuelve `201`, crea movimiento y aumenta existencias.
- **AC-02:** Una salida válida devuelve `201`, crea movimiento y reduce existencias.
- **AC-03:** Una salida con stock insuficiente devuelve `409` sin efectos parciales.
- **AC-04:** Tipo o cantidad inválida devuelve `400` sin efectos parciales.
- **AC-05:** Producto inexistente devuelve `404`.
- **AC-06:** Sin token válido devuelve `401`.
- **AC-07:** Ante fallo transaccional no persiste ninguno de los dos cambios.
- **AC-08:** Operaciones concurrentes no pierden actualizaciones ni generan stock negativo.
- **AC-09:** El movimiento registra fecha UTC, stock anterior y posterior.
