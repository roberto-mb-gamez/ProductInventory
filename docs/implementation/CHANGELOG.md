# Registro de cambios de implementación
Este registro resume cambios de ejecución; Git conserva el historial detallado.

## [2.4.0] — 2026-11-11 — Documentación
- Incorporado plan local de ejecución de las etapas 5.1 a 5.5.
- Añadidos estado, roadmap, registro de decisiones y plantillas de evidencia.
- Añadidos prompts operativos en español para Codex.
- Actualizado AGENTS.md con protocolo de seguimiento.
- Conservados los 17 casos de uso, restricciones y ocho ADR de v2.3.

## Ejecuciones
La ejecución de 5.1 se registra a continuación; las demás etapas no se han ejecutado.

## 2026-11-11 — Etapa 5.1 — COMPLETED, pendiente de aprobación humana
- Creada solución net9.0 con cuatro proyectos fuente y tres de pruebas, referencias según ADR-001.
- Configurados SDK 9.0.321, health checks/DI mínima, configuración por entorno y estructura inicial por funcionalidades.
- TDD de health: rojo 404, verde 200 Healthy. Restore/build correctos; 1 prueba de integración aprobada y comprobación HTTP en Kestrel.
- Documentados arranque, depuración y límites en README; seguimiento actualizado sin avanzar a 5.2.
- Evidencia: [ejecución 5.1](evidence/STAGE-5.1-bootstrap.md). Sin nuevo commit.


## 2026-11-11 — Reorganización autorizada del repositorio
- Agrupados solución, SDK, código fuente y pruebas bajo code/; documentación conservada en raíz/docs/specs.
- Actualizados README y AGENTS.md con rutas y comandos desde code/.
- Restore/build/test correctos tras el traslado; 1 prueba aprobada, 0 errores y advertencias de compilación.
- Evidencia adicional en evidence/STAGE-5.1-bootstrap.md. Etapa 5.1 COMPLETED pendiente de aprobación; sin avanzar de etapa.

## Cambios autorizados — EditorConfig y configuración central
Se incorporaron .editorconfig raíz, Directory.Build.props y Directory.Packages.props bajo code/; global.json selecciona SDK 9.0.321. Eliminadas versiones y propiedades comunes duplicadas de los siete proyectos sin cambiar paquetes. Restore/build/test verificados; detalle en evidencia. Sin avance de etapa ni commit.

## Ajuste autorizado del alcance de EditorConfig
Movido .editorconfig desde la raíz a code/.editorconfig para aplicar sus reglas únicamente dentro de code/. README y seguimiento actualizados; sin cambios en código ni versiones.

## Aprobación humana y preparación de commit
Usuario aprobó explícitamente 5.1 y autorizó commit en español excluyendo docs/implementation/. Se conserva ausencia actual de global.json y se corrigen referencias en README y AGENTS.md. Estado APPROVED; 5.2 no iniciada.

## 2026-11-11 — Etapa 5.2 — COMPLETED, pendiente de aprobación humana
- Verificado prerrequisito 5.1 APPROVED y registrada autorización exclusiva de 5.2.
- Esquema SQL Server reproducible de cuatro tablas, constraints/índices y protección del historial. Inicialización bajo applock/transacción, provisionamiento de base opcional por entorno.
- EF Core 9 para proyecciones de lectura; Dapper para producto con stock cero y movimientos transaccionales seguros ante concurrencia, rollback y cancelación.
- Configurados Dockerfile y Compose base API+SQL, named volume y readiness; imagen SQL fijada a digest verificado. Compose config válido; arranque bloqueado por motores Docker ausentes/helper de credenciales faltante.
- Restore/build correctos, 0 warnings/errors; 28 pruebas de integración SQL reales aprobadas. Startup en Kestrel creó base y cuatro tablas; health 200, ruta de productos 404.
- README e infra/README.md describen ejecución, variables, pruebas y límites. Evidencia y TRX en evidence/STAGE-5.2-infrastructure.md y evidence/test-results-5.2/.
- Sin commit, sin Keycloak ni endpoints funcionales; 5.3 no iniciada. Bases aisladas conservadas para inspección.

## 2026-11-12 — Revalidación Docker de 5.2 — PASSED
- Motor Docker ya accesible; auxiliar de credenciales resuelto mediante PATH temporal, sin cambiar configuración global.
- Dockerfile construido y Compose iniciado: SQL Server healthy y API operativa; .env local generado e ignorado por Git.
- Detectado puerto incorrecto de Kestrel durante smoke; test rojo/verde y corrección de lectura del prefijo ASPNETCORE_ en Program.cs.
- 29 pruebas de integración aprobadas contra SQL Server 16.0.4185.3 del contenedor; /health 200, /api/products 404 y cuatro tablas verificadas.
- Bloqueo anterior cerrado. Evidencia/TRX y documentación actualizados. 5.2 COMPLETED, pendiente de aprobación humana; sin commit ni avance a 5.3.

## 2026-11-12 — Dockerfile junto al proyecto API
Movido Dockerfile a code/src/ProductInventory.Api/Dockerfile; ajustada su ruta en Compose manteniendo contexto code/. README e infra/README.md actualizados. Compose config, build/up y health 200 verificados; sin repetir suite de integración al no cambiar lógica.

## 2026-11-12 — Configuración de pruebas desde Visual Studio
Añadidos plantilla .runsettings, archivo local ignorado con conexión Docker y RunSettingsFilePath condicional en IntegrationTests. Fixture mantiene configuración por entorno. Sin variable de shell, prueba reportada y suite 29/29 pasan con VSTest. infra/README.md y evidencias actualizados.

## 2026-11-12 — Indicaciones de README en español
Traducidos README.md, infra/README.md y el texto restante en inglés de specs/README.md. docs/implementation/prompts/README.md ya estaba en español y se conservó. Comandos, rutas, identificadores y estados técnicos preservados; sin cambiar alcance ni decisiones. Comparación de todos los bloques cercados antes/después idéntica; git diff --check correcto. Sin repetir pruebas por tratarse solo de documentación.

## 2026-11-12 — Aprobación humana de 5.2
Responsable del proyecto aprobó explícitamente 5.2. Verificados COMPLETED previo, 29 pruebas aprobadas/0 fallidas en TRX, SQL healthy y health HTTP 200; sin bloqueos críticos pendientes de la etapa. Registrado APPROVED en STATUS y archivo 5.2; 5.3 queda READY sin autorización ni implementación. README sincronizado. No se realizó commit, push ni merge.

## 2026-11-12 — Commit autorizado de 5.2
Creado commit 9c20362: feat: incorporar persistencia SQL Server y entorno Docker. Incluye 51 archivos; docs/implementation/, .env y archivos *.local.runsettings excluidos. git diff --cached --check correcto antes del commit; contenido del commit comprobado. Sin push ni merge. 5.2 APPROVED y 5.3 READY, no implementada.

## 2026-11-12 — Etapa 5.3 — IN_PROGRESS; validación manual acordada
- Verificado 5.2 APPROVED; integrados Keycloak, realm de desarrollo, JWT Bearer y Swagger Authorization Code + PKCE sin CRUD ni RBAC adicional.
- Por instrucción posterior del usuario, retiradas pruebas nuevas de seguridad, fixtures/emisor y dependencia de Keycloak en el ejecutor de pruebas.
- Suite existente 29/29 aprobada con Keycloak detenido; SQL Server sigue siendo requisito de persistencia. Health usa configuración aislada sin consultar identidad.
- Compose, discovery y Swagger configurados; guía manual en infra/keycloak/README.md. Login/tokens/PKCE reales quedan pendientes de revisión manual antes de COMPLETED.
- No se marca APPROVED ni se inicia 5.4; sin commit, push ni merge.

## 2026-11-12 — Registro de validación, cierre y aprobación de 5.3
Registrada confirmación manual del responsable («la validacion manual funciono al 100%») como evidencia humana. Cerradas T5.3-07 y AC-02/03; registrado y verificado COMPLETED antes de aplicar la aprobación humana explícita previamente otorgada. Estado final 5.3 APPROVED; 5.4 READY sin implementación. STATUS, archivos de etapa, evidencia, README y guía manual sincronizados. Sin pruebas de seguridad nuevas, commit, push ni merge.

## 2026-11-12 — Commit autorizado de 5.3
Creado commit 41247e3: feat: integrar Keycloak y autenticación JWT con Swagger PKCE. Incluye 20 archivos; docs/implementation/, .env y archivos *.local.runsettings excluidos. Índice y contenido final verificados; sin credenciales locales. Sin push ni merge; 5.4 sigue READY sin implementar.

## 2026-11-12 — Inicio autorizado de 5.4 y revisión de contratos
Verificado 5.3 APPROVED y leídos los 17 UC, requisitos, modelo, constraints y ADR. 5.4 IN_PROGRESS; antes de implementar se detectan C5.4-01 (movimientos 200/201) y C5.4-02 (paginación y respuesta de categorías/proveedores sin definir). Se detiene la implementación conforme a AGENTS.md y se registran propuestas para revisión. Solo STATUS, etapa, evidencia y este registro cambiados; sin código, pruebas nuevas ni operaciones Git de escritura.

## 2026-11-12 — 5.4 COMPLETED
Contratos C5.4-01/02 aceptados; implementados 17 UC por grupos, invariantes y CQRS EF/Dapper. 107 pruebas aprobadas, Compose/health/Swagger y protección anónima verificados. Trazabilidad, README y evidencias actualizados. Pendiente aprobación humana; 5.5 sin iniciar, sin commit/push/merge.


## 2026-11-12 — Etapa 5.5 — COMPLETED, pendiente de aprobación final
- Verificada aprobación humana de 5.4 antes de iniciar; alcance limitado a validación y documentación.
- Restore/build de solución correctos; 107/107 pruebas y seis escenarios de concurrencia/rollback/cancelación repetidos sin fallos. Cobertura Coverlet combinada 93.75%, vacíos descritos sin umbral inventado.
- Compose aislado con puertos/volúmenes nuevos; inicialización SQL/realm, salud y Swagger verificados. 57 comprobaciones HTTP con Keycloak real, PKCE y tokens válidos/negativos; sin introducir Keycloak en las suites de pruebas.
- Auditoría de ocho reglas de código, SOLID, dependencias y secretos; trazabilidad 17/17. README y specs/traceability.md actualizados, sin cambios de comportamiento ni contratos.
- Evidencia en evidence/STAGE-5.5-validation.md y evidence/stage-5.5/. Entorno aislado detenido, volúmenes retenidos. Sin commit, publicación, push, merge ni aprobación automática.

## 2026-11-12 — Aprobación humana de 5.5
Verificados COMPLETED y ausencia de bloqueos críticos pendientes; registrada la aprobación explícita del responsable en STATUS.md y STAGE-5.5-validation.md. Estado APPROVED. El roadmap no define etapa posterior: siguiente etapa READY pendiente de alcance/numeración, sin crear ni implementar una etapa adicional. Sin commit, push, merge ni publicación.

## 2026-11-12 — Migración autorizada de writers a procedimientos
Implementados diez procedimientos SQL con llamadas Dapper parametrizadas, manteniendo readers EF y contratos de los 17 UC. Inicialización ordenada/atómica con CREATE OR ALTER sobre bases nuevas y existentes; movimientos con transacción en SQL, UTC y resultados explícitos. Dos pruebas rojas registradas antes de implementar; cierre con 112/112 pruebas en Release, cobertura 91.84%, auditoría de código y Compose/Swagger/health correctos. Base Compose conserva los conteos de datos e instala diez procedimientos. ADR-002/003 sincronizados en inglés/español y documentación actualizada. Evidencia en evidence/STORED-PROCEDURES.md. Sin commit, push, merge ni aprobación automática de esta mejora.
