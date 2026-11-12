# STAGE-5.3 — Seguridad OAuth2

**Estado:** APPROVED
**Aprobación humana:** CONFIRMADA explícitamente por el responsable del proyecto el 2026-11-12, después del registro de cierre COMPLETED.
**Prerrequisito:** 5.2 APPROVED por el responsable del proyecto el 2026-11-12.
**Autorización para ejecutar 5.3:** CONFIRMADA explícitamente por el usuario el 2026-11-12; únicamente seguridad, sin CRUD ni RBAC adicional.
**Documentos aplicables:** ADR-005, ADR-006, ADR-007, ADR-008

## Objetivo
Seguridad OAuth2 respetando la línea base funcional y técnica aprobada.

## Tareas
- [x] T5.3-01: Configurar realm de desarrollo Keycloak, cliente Swagger público con PKCE y audiencia API.
- [x] T5.3-02: Agregar servicio Keycloak a Compose e importación reproducible de realm.
- [x] T5.3-03: Configurar JWT Bearer con issuer, audience, firma, expiración y validación de tokens.
- [x] T5.3-04: Proteger endpoints de negocio; mantener health endpoint con política explícita.
- [x] T5.3-05: Configurar Swagger UI con Authorization Code + PKCE y redirect URI correcto.
- [x] T5.3-06: Resolver coherencia del issuer entre navegador, API en contenedor y ejecución local.
- [x] T5.3-07: Validar manualmente, ejecutando la aplicación, 401 para token ausente/expirado/inválido y acceso con token válido; sin pruebas automatizadas de seguridad por instrucción explícita del usuario.
- [x] T5.3-08: Documentar variables de entorno, credenciales exclusivamente de desarrollo y obtención de token.

## Criterios de aceptación
- [x] AC-01: Compose completo levanta API, SQL Server y Keycloak.
- [x] AC-02: Validación JWT real sin desactivar issuer/audience/signature.
- [x] AC-03: Swagger PKCE y validación manual de autenticación documentados y comprobados, sin depender de Keycloak en la suite automatizada.
- [x] AC-04: Sin implementación de gestión de usuarios ni RBAC adicional.

## Evidencia de ejecución
Implementación y arranque ejecutados el 2026-11-12. Compose integra API, SQL Server y Keycloak. Restore/build correctos; suite existente 29/29 aprobada incluso con Keycloak detenido. Pruebas nuevas de seguridad retiradas y configuración de pruebas desacoplada por instrucción explícita del usuario. Validación manual confirmada satisfactoria por el responsable del proyecto el 2026-11-12: «la validacion manual funciono al 100%». Evidencia de origen humano, sin afirmar ejecución automatizada por Codex. Ver ../evidence/STAGE-5.3-security.md.

## Riesgos y bloqueos
Sin bloqueos críticos pendientes: implementación y arranque verificados, validación manual confirmada por el responsable y registrada. La suite de persistencia no se usa como sustituto de la validación manual JWT/PKCE. Sin CRUD ni RBAC adicional.

## Revisión
El responsable del proyecto aprobó explícitamente 5.3 tras informar validación manual satisfactoria. Se registró primero la evidencia humana y el cierre COMPLETED, se verificaron todas las tareas y criterios terminados y la ausencia de bloqueos críticos pendientes, y después se aplicó la aprobación ya otorgada. La etapa 5.4 queda READY sin autorización de implementación. No se autorizaron commit, push ni merge.
