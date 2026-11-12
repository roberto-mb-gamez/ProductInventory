# ProductInventory — Roadmap de ejecución (Etapa 5)
**Versión:** 2.4 | **Estado:** Preparado para ejecución | **Idioma:** Español (documentación de gestión)

## Principios
- Los requisitos, los 17 casos de uso y los ocho ADR aceptados de v2.3 son la línea base.
- No cambiar requisitos, arquitectura o versiones sin autorización del usuario.
- Codex implementa y documenta; el usuario aprueba el paso entre etapas.
- `COMPLETED` significa trabajo verificado por Codex; `APPROVED` requiere confirmación explícita del usuario.
- Nunca interpretar una casilla marcada como evidencia de ejecución: anotar comando, resultado, fecha y referencia de commit cuando exista.
- Seguir TDD en cada funcionalidad; registrar primero las pruebas y sus resultados fallidos esperados cuando aplique.
- Evitar commits, pushes, merges, creación de PR y operaciones destructivas sin permiso explícito.

## Etapas y dependencias
| Etapa | Objetivo | Prerrequisito | Entregable / puerta de salida |
|---|---|---|---|
| 5.1 | Bootstrap .NET 9 | Documentación v2.4 | Solución compilable, proyectos y referencias correctas, pruebas iniciales, health endpoint |
| 5.2 | Persistencia e infraestructura | 5.1 APPROVED | SQL Server, esquema/migraciones, EF Core lecturas, Dapper escrituras, transacción de movimientos, Compose base |
| 5.3 | Seguridad | 5.2 APPROVED | Keycloak, JWT Bearer, Swagger PKCE, entorno Compose completo, pruebas 401 y token válido |
| 5.4 | Casos de uso con TDD | 5.3 APPROVED | 17 UC, endpoints, validaciones, pruebas unitarias e integración por UC |
| 5.5 | Validación y entrega | 5.4 APPROVED | Pruebas de extremo a extremo, concurrencia, cobertura, README, auditoría, entrega lista para revisión |

## Etapa 5.0 — Preparación de seguimiento
Los archivos de seguimiento y prompts quedan preparados con esta versión. No se ha ejecutado código de aplicación.
La primera etapa autorizable es 5.1; el estado inicial es READY, no IN_PROGRESS.

## Protocolo por sesión
1. Leer `AGENTS.md`, `constraints.md`, ADR pertinentes, `STATUS.md`, `ROADMAP.md` y el archivo de etapa.
2. Inspeccionar `git status`, rama y archivos reales; contrastar con el seguimiento.
3. Trabajar solo en la etapa autorizada, por tareas pequeñas.
4. Ejecutar las verificaciones posibles y guardar resultados exactos.
5. Actualizar la lista de tareas, `STATUS.md`, `CHANGELOG.md` y `DECISIONS.md` si procede.
6. Si una etapa está COMPLETED, detenerse y solicitar aprobación humana antes de la siguiente.

## Git
Se recomienda GitHub Flow: `feature/stage-5.1-bootstrap` y sucesivas, creadas desde `main` actualizado.
No crear rama ni hacer commit sin autorización; registrar referencias cuando existan.
Usar Conventional Commits. Para 5.4, preferir commits por UC o grupo pequeño.
