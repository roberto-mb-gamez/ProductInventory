# Registro de decisiones menores de implementación
Los ADR aceptados en `docs/adr/` tienen prioridad. Este archivo **no** puede modificar ADR, requisitos o alcance.

| ID | Fecha | Etapa | Decisión menor | Motivo | Alternativas | Estado |
|---|---|---|---|---|---|
| D-001 | 2026-11-11 | 5.1 | Fijar SDK 9.0.321 con latestPatch; mantener net9.0 | SDK 9 disponible y compilación reproducible | SDK implícito 10 | Implementada |
| D-002 | 2026-11-11 | 5.1 | Usar AddHealthChecks/MapHealthChecks como DI mínima; reservar Features en Api/Application | Sin servicios ficticios ni dependencias externas; no exponer rutas de negocio | Registro vacío por capa | Implementada |
| D-003 | 2026-11-11 | 5.1 | Mantener proyectos unitarios sin pruebas hasta implementar comportamiento | Evitar pruebas triviales de plantilla; validar health con WebApplicationFactory | Pruebas vacías | Implementada |

Si una decisión afecta arquitectura, seguridad, alcance o contratos aprobados: detenerse y solicitar autorización; no registrarla aquí como sustituto de un ADR.


Reorganización autorizada por el usuario el 2026-11-11: agrupar solución, global.json, src/ y tests/ en code/. README y AGENTS.md reflejan el directorio de ejecución. No cambia ADR-001 ni la arquitectura.

Decisión menor autorizada antes de aprobar 5.1: usar EditorConfig para formato, Directory.Build.props para framework y propiedades comunes, Directory.Packages.props para versiones NuGet y global.json para SDK. Se mantienen net9.0 y las seis versiones de paquetes existentes. No cambia arquitectura ni requiere ADR nuevo. EditorConfig no sustituye la revisión manual de las reglas especiales del ADR-008.

Por preferencia explícita del usuario, EditorConfig queda en code/.editorconfig y limita sus reglas al árbol code/. Se conserva root=true para no heredar configuraciones superiores.

## Decisiones menores — Etapa 5.2
| ID | Fecha | Etapa | Decisión menor | Motivo | Alternativas | Estado |
|---|---|---|---|---|---|---|
| D-004 | 2026-11-11 | 5.2 | GUID como PK; int para stock/cantidad; decimal(19,4) para precio; datetimeoffset(7) UTC | Tipos de persistencia definidos en ADR-003 como detalle de implementación; sin alterar reglas de negocio | IDs numéricos, otras precisiones | Implementada y probada |
| D-005 | 2026-11-11 | 5.2 | Script SQL baseline idempotente embebido, con transacción y applock; inicialización opcional por entorno | Cuatro tablas exactas y esquema reproducible sin usar EF para escrituras de negocio | EF migrations | Implementada y probada; cambios futuros requieren scripts explícitos |
| D-006 | 2026-11-11 | 5.2 | ReadModels EF exclusivos de Infrastructure, contexto sin tracking y SaveChanges deshabilitado | Mantener Domain independiente y separar lecturas/escrituras | Mapear futuras entidades de dominio directamente | Implementada y probada |
| D-007 | 2026-11-11 | 5.2 | UPDATE condicional con OUTPUT e intermedio bigint, seguido de INSERT en misma SqlTransaction | Evitar stock negativo, overflow y pérdida de actualizaciones; rollback al disponer transacción no confirmada | Lectura bloqueada seguida de actualización | Implementada y probada |
| D-008 | 2026-11-11 | 5.2 | Tests sobre SQL local real en bases únicas, retenidas para inspección | Docker no operativo; verificar semántica real sin modificar datos existentes | Esperar Docker; no se usa SQLite ni EF InMemory | Implementada, limitación de contenedores registrada |
| D-009 | 2026-11-11 | 5.2 | Compose raíz; Dockerfile/contexto en code/; placeholders/documentación en infra/; SQL host 14333 | Respetar estructura acordada y evitar conflicto con SQL local | Puerto host 1433 | Preparada; runtime Docker bloqueado |
No se cambian ADR ni contratos funcionales. La alternativa de AC-04 permite cerrar 5.2 con bloqueo verificable de Docker; no implica que la ejecución en contenedores esté validada.

D-008/D-009 revalidadas el 2026-11-12: SQL Server del contenedor disponible, 29 pruebas aprobadas y Compose/Dockerfile/health verificados. Se cierra la limitación de runtime Docker sin cambios de arquitectura. La configuración de entorno de Api conserva el prefijo ASPNETCORE_ para respetar ASPNETCORE_HTTP_PORTS; corrección cubierta por test rojo/verde.

Cambio autorizado el 2026-11-12: Dockerfile junto al proyecto API en code/src/ProductInventory.Api/Dockerfile. Compose mantiene contexto ./code y usa dockerfile: src/ProductInventory.Api/Dockerfile; .dockerignore permanece en code/. Actualiza la ubicación de D-009 sin cambiar arquitectura.

## Decisiones de 5.3 y estrategia autorizada de validación
- Emisor público fijo http://localhost:8081/realms/productinventory; discovery de API en Docker por keycloak:8080 y backchannel dinámico. Swagger usa endpoints públicos; no se altera ValidIssuer para admitir direcciones internas.
- Identidad y Swagger requieren configuración por entorno; HTTPS fuera de Development, RS256, ClockSkew 30 segundos y política global autenticada. /health es explícitamente anónimo.
- Keycloak en Compose se conserva para ejecutar la aplicación y revisarla manualmente. La suite automatizada conserva solo las 29 pruebas existentes y no necesita Keycloak ni su contenedor; sus clases de seguridad nuevas fueron retiradas por instrucción explícita del usuario. Esta instrucción prevalece sobre la estrategia inicial automatizada de T5.3-07.
- Estado 5.3 IN_PROGRESS hasta registrar validación manual efectiva. No se modifica arquitectura ni ADR; no se agrega RBAC ni CRUD.

## Contratos 5.4 aprobados explícitamente — 2026-11-12
El responsable aceptó C5.4-01 y C5.4-02: UC-006 devuelve 201 Created; UC-010/015 usan page=1, pageSize=20, máximo 100 y respuesta items/page/pageSize/totalCount, con 400 ante parámetros inválidos. Catálogo y UC sincronizados. Continúa la autorización general de 5.4 por grupos; seguridad permanece manual y tests sin Keycloak.

## Validación de 5.5 — 2026-11-12
- Se usa proyecto Compose productinventory-stage55 con puertos 5281/8181/15333, contraseñas temporales y volúmenes nuevos para comprobar inicialización limpia sin eliminar datos existentes. Al terminar se detiene sin -v.
- Se compila la solución completa con MSBuild de Visual Studio por la inclusión de Microsoft.Docker.Sdk; las suites se ejecutan seleccionando cada csproj y suprimiendo la conexión de runsettings local para apuntar al SQL aislado.
- La validación HTTP puntual de seguridad queda fuera de los proyectos xUnit y usa Authorization Code/PKCE; no modifica validadores ni habilita password grant en Swagger. No se incorpora una nueva dependencia de Keycloak al ejecutor de pruebas. CookieJar se adapta únicamente a cookies Secure de HTTP localhost en el cliente temporal.
- No se cambia arquitectura, stack, contrato ni alcance. Cobertura reportada sin umbral; publicación y aprobación final retenidas por instrucción del usuario.
