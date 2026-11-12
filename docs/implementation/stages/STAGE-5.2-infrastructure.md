# STAGE-5.2 — Infraestructura y persistencia

**Estado:** APPROVED
**Aprobación humana:** CONFIRMADA explícitamente por el responsable del proyecto el 2026-11-12.
**Documentos aplicables:** ADR-002, ADR-003, ADR-007, ADR-008

## Objetivo
Infraestructura y persistencia respetando la línea base funcional y técnica aprobada.

## Tareas
- [x] T5.2-01: Definir modelo SQL Server de Products, Categories, Suppliers e InventoryMovements con PK/FK, restricciones e índices.
- [x] T5.2-02: Crear mecanismo reproducible de migración/inicialización del esquema.
- [x] T5.2-03: Configurar DbContext y mapeos de lectura EF Core 9; proyecciones AsNoTracking.
- [x] T5.2-04: Configurar conexiones y contratos de escritura Dapper, SQL parametrizado y manejo de transacciones.
- [x] T5.2-05: Implementar componente transaccional de movimiento con actualización condicional de stock y rollback; sin exponer todavía UC-006 como endpoint de negocio.
- [x] T5.2-06: Preparar Dockerfile de API y Compose base con API y SQL Server, dejando la integración final de Keycloak para 5.3.
- [x] T5.2-07: Probar lecturas reales EF Core, escrituras Dapper, integridad referencial, rollback y concurrencia SQL.
- [x] T5.2-08: Registrar resultados de migración, health checks, variables de entorno y limitaciones.

## Criterios de aceptación
- [x] AC-01: Esquema SQL Server reproducible con cuatro tablas.
- [x] AC-02: EF Core usado para consultas y Dapper para escrituras de negocio.
- [x] AC-03: Movimiento y stock atómicos bajo concurrencia y pruebas reales.
- [x] AC-04: Compose base arranca API y SQL Server o registra bloqueo verificable.

## Evidencia de ejecución
Ejecutado el 2026-11-11 y revalidado en Docker el 2026-11-12. Ver [evidencia por tarea y resultados](../evidence/STAGE-5.2-infrastructure.md) y TRX bajo ../evidence/test-results-5.2/. Restore/build iniciales correctos; Dockerfile construido/publicado en Release y Compose iniciado correctamente. 29 pruebas de integración aprobadas contra SQL Server del contenedor; /health HTTP 200 y cuatro tablas verificadas. Bloqueo previo de Docker resuelto. TRX adicionales en ../evidence/test-results-5.2-docker/.

## Riesgos y bloqueos
Sin bloqueos actuales de Docker. Se resolvió el PATH del auxiliar de credenciales para la sesión y se corrigió el prefijo ASPNETCORE_ para respetar el puerto del contenedor. Bases y volumen retenidos; .env ignorado por Git. Aprobación humana registrada el 2026-11-12.

## Revisión
El responsable del proyecto aprobó explícitamente la etapa después de revisar sus evidencias. Antes de registrar la aprobación se verificó COMPLETED, todas las tareas y criterios marcados, TRX con 29 pruebas aprobadas y 0 fallidas, SQL Server healthy y /health HTTP 200 Healthy. Sin bloqueos críticos pendientes para 5.2. La etapa 5.3 queda READY, sin autorización de implementación. No se autorizó commit, push ni merge.

## Ubicación actual del Dockerfile
code/src/ProductInventory.Api/Dockerfile, con contexto de construcción code/ y .dockerignore en code/. Traslado autorizado y verificado con Compose build/up y HTTP health 200.
