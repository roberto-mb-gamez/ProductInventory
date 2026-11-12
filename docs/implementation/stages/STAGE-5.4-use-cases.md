# STAGE-5.4 — Implementación de los 17 casos de uso con TDD

**Estado:** COMPLETED
**Aprobación humana:** CONFIRMADA explícitamente el 2026-11-12
**Prerrequisito:** 5.3 APPROVED por el responsable del proyecto el 2026-11-12.
**Autorización para ejecutar 5.4:** CONFIRMADA explícitamente por el usuario el 2026-11-12; únicamente los 17 UC, TDD por grupos, EF lecturas y Dapper escrituras.
**Documentos aplicables:** ADR-001 a ADR-008; specs/usecases/UC-001 a UC-017

## Objetivo
Implementación de los 17 casos de uso con TDD respetando la línea base funcional y técnica aprobada.

## Tareas
- [x] T5.4-01: UC-007: verificar acceso protegido transversal a los endpoints.
- [x] T5.4-02: UC-008 a UC-012: CRUD de Category, unicidad y bloqueo de borrado con productos.
- [x] T5.4-03: UC-013 a UC-017: CRUD de Supplier y bloqueo de borrado con productos.
- [x] T5.4-04: UC-001 a UC-005: CRUD de Product, SKU único, relaciones y bloqueo de borrado con historial.
- [x] T5.4-05: UC-003: filtros, paginación y proyecciones con Category/Supplier.
- [x] T5.4-06: UC-006: endpoint POST /api/products/{id}/movements para IN/OUT; verificar 201/400/404/409.
- [x] T5.4-07: Escribir pruebas primero y registrar evidencias de rojo/verde/refactor para cada UC.
- [x] T5.4-08: Completar OpenAPI y respuestas de error coherentes; revisar que todas las consultas usen EF y las escrituras Dapper.
- [x] T5.4-09: Actualizar matriz de trazabilidad UC ↔ pruebas y verificar 17/17.

## Criterios de aceptación
- [x] AC-01: Los 17 UC están implementados conforme a las especificaciones aprobadas.
- [x] AC-02: Las reglas de stock, historial y borrado son consistentes.
- [x] AC-03: Pruebas unitarias e integración relevantes pasan o sus bloqueos se registran.
- [x] AC-04: No se agregan endpoints o entidades fuera de alcance.

## Evidencia de ejecución
Implementados 17/17 UC. Restore, build y 107 pruebas aprobados; Compose reconstruido y API arrancada, health 200, Swagger 16 operaciones y acceso sin token 401 en todas. Evidencia de rojo/verde por grupo y resultados TRX en ../evidence/STAGE-5.4-use-cases.md. UC-007 conserva validación manual aprobada en 5.3; no se agregan pruebas automatizadas de Keycloak.

## Riesgos y bloqueos
Resueltas por aceptación explícita del responsable el 2026-11-12: C5.4-01, UC-006 devuelve 200 según catálogo o 201 según UC y plan; C5.4-02, concretar paginación/respuesta de UC-010 y UC-015 frente a NFR-007/BR-020. Recomendación: 201 Created y listados paginados con page=1, pageSize=20, límite 100 y respuesta items/page/pageSize/totalCount. Contratos aprobados y sincronizados; continúa TDD por grupos. Se conserva la decisión previa de seguridad manual, sin dependencia de Keycloak en pruebas.

## Revisión
Aprobada explícitamente por el responsable del proyecto el 2026-11-12 después de verificar COMPLETED y ausencia de bloqueos críticos. Estado APPROVED registrado por aprobación humana; no por Codex.

## Cierre — 2026-11-12
COMPLETED, nunca APPROVED por Codex. Sin bloqueos críticos pendientes. No se inició 5.5 ni se realizaron commit, push o merge. Requiere revisión y aprobación humana explícita.


## Aprobación humana — 2026-11-12
El responsable aprobó explícitamente 5.4 tras revisar sus evidencias. Antes del cambio se verificó COMPLETED, 17/17 UC y cero bloqueos críticos pendientes. 5.4 queda APPROVED; 5.5 pasa a READY y permanece sin iniciar. Sin commit, push ni merge.


