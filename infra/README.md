# Entorno de persistencia local
El entorno incorpora API, SQL Server y Keycloak durante la etapa 5.3. La etapa 5.4 incorpora las 16 operaciones HTTP de negocio protegidas. La [guía de seguridad](keycloak/README.md) describe la revisión manual; la suite automatizada no depende de Keycloak. Conserva los secretos en variables de entorno o en el archivo `.env` de la raíz, ignorado por Git; el `.env.example` incluido contiene únicamente valores de ejemplo para sustituir.
## Docker Compose
Desde la raíz del repositorio:
```powershell
Copy-Item infra/.env.example .env
```
Si `.env` ya existe, conserva sus valores y agrega solo las variables nuevas de `.env.example`. Sustituye los marcadores de contraseña SQL, administrador de Keycloak y usuario de desarrollo por contraseñas locales diferentes. Utiliza una contraseña sin separadores de cadenas de conexión, como punto y coma o comillas. Después ejecuta:
```powershell
docker compose --env-file .env up --build -d
docker compose ps
Invoke-WebRequest http://localhost:5181/health
```
Si acabas de instalar Docker Desktop, reinicia la terminal para que reciba el PATH actualizado. En instalaciones por usuario, puede ser necesario un ajuste temporal de PowerShell para localizar el auxiliar de credenciales:
```powershell
$env:PATH = "$env:LOCALAPPDATA\Programs\DockerDesktop\resources\bin;$env:PATH"
Get-Command docker-credential-desktop
```
Este cambio afecta únicamente a la terminal actual. El motor Linux de Docker debe estar funcionando; `docker version` debe mostrar Client y Server.
La API se construye desde `code/src/ProductInventory.Api/Dockerfile`, con `code/` como contexto de construcción y `code/.dockerignore` como archivo de exclusiones. SQL Server utiliza la imagen verificada 2022 CU18 Ubuntu 22.04, fijada por su resumen de contenido (digest), y almacena datos en `sqlserver-data`. SQL se expone de forma predeterminada en el puerto 14333 del equipo, para evitar el puerto habitual del SQL Server local. La API espera la comprobación de disponibilidad de SQL e inicializa la base y el esquema antes de quedar accesible en el puerto 5181 del equipo. Un fallo de inicialización impide el arranque de la API. Conserva el volumen con nombre al detener el entorno; `docker compose down` detiene los servicios sin eliminarlo.
Esta configuración es para desarrollo: utiliza la edición Developer, credenciales SA para crear la base y TrustServerCertificate en el entorno local. Keycloak utiliza start-dev y almacenamiento embebido persistente para esta evaluación; no se configura identidad de producción ni credenciales de despliegue con privilegios mínimos. Las imágenes del SDK y del entorno de ejecución de .NET usan etiquetas 9.0; el digest de la imagen SQL está fijado.
## Variables de entorno
| Variable | Propósito | Valor predeterminado |
|---|---|---|
| SQL_SERVER_PASSWORD | Contraseña SQL de desarrollo requerida por Compose | Sin valor predeterminado |
| SQL_SERVER_PORT | Puerto SQL en el equipo | 14333 |
| API_PORT | Puerto de la API en el equipo | 5181 |
| ConnectionStrings__Inventory | Cadena de conexión SQL de la API | Sin conexión; solo salud del proceso |
| Database__Initialize | Crear la base indicada si no existe e inicializar el esquema antes de servir HTTP | false; Compose establece true |
| ASPNETCORE_URLS / ASPNETCORE_HTTP_PORTS | Dirección o puerto de escucha de la API | Se establece en la terminal local o el contenedor |
| KEYCLOAK_ADMIN_PASSWORD | Contraseña local de administración de Keycloak | Requerida por Compose |
| KEYCLOAK_DEVELOPMENT_PASSWORD | Contraseña del usuario inventory-developer | Requerida por Compose |
| KEYCLOAK_PORT | Puerto público de Keycloak | 8081 |
| Identity__Authority | Emisor público que debe coincidir con iss del token | Requerido para iniciar la API |
| Identity__MetadataAddress | Discovery interno en Docker o público en depuración local | Derivado del emisor si se omite |
| Identity__Audience | Audiencia válida de la API | Requerida; Compose usa productinventory-api |
| Swagger__Enabled / Swagger__ClientId | Habilitación de Swagger y cliente público PKCE | false / requerido al habilitar Swagger |
| PRODUCTINVENTORY_TEST_CONNECTION_STRING | Conexión SQL con permiso de creación de bases para las pruebas de integración | Requerida para ejecutar pruebas de base de datos |
Para depurar la API localmente, establece ConnectionStrings__Inventory y Database__Initialize en el entorno del depurador y selecciona ProductInventory.Api como proyecto de inicio. Para conectarte al servicio SQL de Compose desde el equipo, utiliza `Server=localhost,14333;Database=ProductInventory;User ID=sa;Password=<local-password>;Encrypt=true;TrustServerCertificate=true`.
## Esquema y permisos
El esquema está en `code/src/ProductInventory.Infrastructure/Persistence/Schema/001_initial.sql`, embebido en Infrastructure. El inicializador lo ejecuta mediante Dapper. Crea exactamente Categories, Suppliers, Products e InventoryMovements. SQL aplica claves primarias GUID, unicidad de SKU y nombre de categoría sin distinguir mayúsculas, claves foráneas sin eliminación en cascada, cantidades positivas, aritmética válida de existencias y desfase horario UTC. Un desencadenador de actualización y eliminación protege el historial de movimientos. Las escrituras de productos omiten StockQuantity para que SQL lo inicialice en cero.
La inicialización utiliza bloqueos de aplicación SQL y una transacción; repetirla conserva los datos. Es un mecanismo idempotente de esquema inicial y no migra automáticamente diferencias arbitrarias en esquemas existentes. Los cambios futuros requieren scripts explícitos de migración y pruebas. Price es decimal(19,4); las existencias y cantidades son int; las fechas son datetimeoffset(7) en UTC; las descripciones y correos opcionales permanecen como texto sin longitud máxima definida.
La creación de la base utiliza una conexión independiente a master con el identificador de base delimitado, rechaza bases de sistema y libera su bloqueo de aplicación. No se eliminan bases existentes. La creación durante el arranque requiere permisos CREATE DATABASE y DDL; si está deshabilitada, proporciona un esquema existente y los permisos SELECT/INSERT/UPDATE correspondientes. Las escrituras de negocio utilizan SQL parametrizado con Dapper; SaveChanges de EF Core está deshabilitado en el contexto de lectura. No se reintentan escrituras automáticamente. Las operaciones elegidas como víctimas de un interbloqueo producen ConcurrencyConflict; los demás fallos de persistencia se propagan después de que la liberación de la transacción revierta las escrituras sin confirmar.
Si aplicas el esquema manualmente a una base de desarrollo ya creada con autenticación de Windows, ejecuta desde la raíz del repositorio:
```powershell
sqlcmd -S localhost -E -C -d ProductInventory -b -i code/src/ProductInventory.Infrastructure/Persistence/Schema/001_initial.sql
```
Las comprobaciones de salud verifican la conectividad mediante SELECT 1, no la integridad del esquema. Con SQL configurado, GET /health devuelve 200 Healthy o 503 Unhealthy sin incluir detalles de la conexión.
## Pruebas de integración con SQL real
Ejecuta desde `code/`:
```powershell
$env:PRODUCTINVENTORY_TEST_CONNECTION_STRING = 'Server=localhost;Integrated Security=true;Encrypt=true;TrustServerCertificate=true'
dotnet restore ProductInventory.sln
dotnet build ProductInventory.sln --no-restore
dotnet test ProductInventory.sln --no-build --no-restore
```
Utiliza credenciales SQL del entorno para conectarte a la instancia SQL de Compose. Las pruebas nunca sustituyen SQL Server por SQLite ni por un proveedor en memoria. Cada ejecución crea ProductInventoryTests_<guid> y una base independiente ProductInventoryStartupTests_<guid>, conservadas para inspección y limpieza manual. Los desencadenadores exclusivos de pruebas provocan un fallo de inserción y una cancelación controlada después de actualizar las existencias. Las pruebas de entradas y salidas concurrentes usan conexiones SQL independientes y una señal de inicio compartida.
### Explorador de pruebas de Visual Studio
Una variable definida mediante `$env:...` en una terminal no está disponible automáticamente en un Visual Studio que ya estaba abierto. Copia `code/integration.runsettings.example` a `code/integration.local.runsettings` y sustituye el marcador de contraseña por la contraseña SQL de desarrollo local. También puedes usar una conexión SQL local con autenticación de Windows. El archivo local está ignorado por Git y debe permanecer sin seguimiento. Proporciona PRODUCTINVENTORY_TEST_CONNECTION_STRING al proceso de pruebas como variable de entorno; el fixture sigue leyendo su configuración únicamente del entorno.
El proyecto de integración establece RunSettingsFilePath de forma condicional para ese archivo local, de modo que el Explorador de pruebas pueda cargarlo automáticamente. Recarga o recompila el proyecto después de crearlo, mantén SQL Server en ejecución y vuelve a ejecutar las pruebas. El valor de runsettings tiene prioridad sobre una variable de la terminal; modifica o renombra temporalmente el archivo local al probar otro servidor. Sin ese archivo, se mantiene la configuración por variables del entorno de la terminal o de integración continua.
RunSettingsFilePath y EnvironmentVariables están documentados en la [guía de runsettings de Microsoft](https://learn.microsoft.com/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file). Para seleccionar el archivo manualmente, utiliza Test > Configure Run Settings > Select Solution Wide runsettings File, o sus equivalentes en la interfaz traducida de Visual Studio. Comando equivalente desde code/: `dotnet test tests/ProductInventory.IntegrationTests --settings integration.local.runsettings`.
Validación inicial: 28 pruebas de integración sobre SQL Server local 15.0.2210.4; inicialización del esquema, restricciones referenciales, de unicidad y de validación, proyecciones relacionales EF, existencias iniciales en cero, atomicidad IN/OUT, existencias insuficientes, desbordamiento de enteros, rollback, cancelación, entradas y salidas concurrentes, historial inmutable, creación de la base durante el arranque y salud SQL. La validación de 5.4 suma 20 pruebas de Domain, 23 de Application y 64 de integración, todas aprobadas.
La validación en contenedores terminó el 2026-11-12: la construcción del Dockerfile y el arranque de Compose fueron satisfactorios; SQL Server 16.0.4185.3 está saludable, la base de la aplicación contiene las cuatro tablas esperadas y http://localhost:5181/health devuelve 200 Healthy. Las 29 pruebas de integración pasaron contra SQL Server del contenedor, incluida una prueba de regresión de ASPNETCORE_HTTP_PORTS. La configuración de entorno de la API conserva el reconocimiento del prefijo ASPNETCORE_ para que el contenedor escuche en el puerto 8080. Los bloqueos anteriores del motor Docker y del auxiliar de credenciales quedaron resueltos para esta validación. El `.env` local contiene una contraseña de desarrollo generada, está ignorado por Git y debe permanecer sin seguimiento.

Las 29 pruebas existentes también se verificaron con Keycloak detenido. Los únicos requisitos de las pruebas de persistencia siguen siendo SQL Server y su conexión de pruebas; inicia solo SQL con `docker compose up -d sqlserver` si no vas a ejecutar la aplicación. La configuración del Explorador de pruebas no contiene credenciales ni conexiones de Keycloak. Los casos de login, tokens y PKCE se revisan manualmente según `keycloak/README.md`.

Validación 5.4: Compose reconstruido e iniciado, health 200 Healthy, Swagger con 16 operaciones; las 16 rechazan solicitudes sin token con 401. La suite de negocio usa autenticación exclusiva del host de pruebas y SQL Server real, sin contactar Keycloak.
