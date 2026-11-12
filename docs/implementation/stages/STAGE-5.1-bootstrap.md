# STAGE-5.1 — Bootstrap de la solución

**Estado:** APPROVED
**Aprobación humana:** CONFIRMADA explícitamente por el usuario
**Documentos aplicables:** ADR-001, ADR-004, ADR-005, ADR-008

## Objetivo
Bootstrap de la solución respetando la línea base funcional y técnica aprobada.

## Tareas
- [x] T5.1-01: Inspeccionar repositorio y verificar disponibilidad de .NET 9 sin modificar código.
- [x] T5.1-02: Crear ProductInventory.sln y proyectos Api, Application, Domain, Infrastructure.
- [x] T5.1-03: Crear Domain.Tests, Application.Tests e IntegrationTests con xUnit y Moq.
- [x] T5.1-04: Configurar referencias y comprobar que Domain no depende de capas externas.
- [x] T5.1-05: Configurar DI mínima y endpoint GET /health sin lógica de negocio.
- [x] T5.1-06: Preparar estructura por funcionalidades sin implementar UC.
- [x] T5.1-07: Ejecutar dotnet restore, dotnet build y dotnet test y registrar resultados.
- [x] T5.1-08: Documentar cómo iniciar y depurar el bootstrap localmente.

## Criterios de aceptación
- [x] AC-01: Compilación satisfactoria con SDK .NET 9 (o bloqueo registrado).
- [x] AC-02: Referencias respetan ADR-001 y no existen dependencias circulares.
- [x] AC-03: Pruebas iniciales ejecutables y health endpoint comprobado cuando sea posible.
- [x] AC-04: Sin persistencia, OAuth2, Docker ni lógica de casos de uso todavía.

## Evidencia de ejecución
Ejecutado el 2026-11-11 con autorización explícita. Evidencia por tarea, comandos y resultados: [STAGE-5.1-bootstrap](../evidence/STAGE-5.1-bootstrap.md). SDK 9.0.321; restore y build satisfactorios; 1 prueba de integración aprobada y health HTTP real 200 Healthy. Domain.Tests y Application.Tests aún sin pruebas.

## Riesgos y bloqueos
Sin bloqueos pendientes para bootstrap. Restricciones iniciales de permisos y NuGet resueltas con autorización; ver evidencia. Inconsistencias funcionales existentes pendientes antes de implementar negocio.

## Revisión
Usuario confirmó que la etapa 5.1 es correcta y autorizó su commit excluyendo docs/implementation/. Aprobación registrada; no implica autorización para iniciar 5.2.



## Ubicación después de la reorganización autorizada
La solución está en `code/ProductInventory.sln`; fuentes y pruebas bajo `code/src/` y `code/tests/`. Ejecutar los comandos .NET desde `code/`. Restore, build y prueba de health verificados de nuevo; ver evidencia de reorganización.

## Configuración central añadida antes de aprobación
code/.editorconfig (limitado a code/) y archivos code/Directory.Build.props, code/Directory.Packages.props y code/global.json controlan formato, framework, versiones NuGet y SDK. Restore/build/test repetidos satisfactoriamente; evidencia ampliada. Estado COMPLETED, aprobación humana pendiente.

