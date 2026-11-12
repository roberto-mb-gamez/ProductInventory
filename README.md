# ProductInventory — API de inventario con .NET 9
La etapa 5.1 está APPROVED. La etapa 5.2 de persistencia está APPROVED por el responsable del proyecto, con 29 pruebas de integración aprobadas contra SQL Server en Docker. La etapa 5.3 está APPROVED: JWT, Keycloak y Swagger PKCE están configurados y el responsable confirmó satisfactoria la validación manual. La etapa 5.4 está READY, sin autorización para implementarla. Se verificaron la construcción del Dockerfile, el arranque de Compose, la inicialización del esquema y la comprobación de salud HTTP. Los endpoints de negocio y CRUD todavía no se implementaron.
## Requisitos
Instala un SDK de .NET capaz de compilar para net9.0 y el entorno de ejecución de ASP.NET Core 9. La estructura inicial se verificó con el SDK 9.0.321. La primera restauración requiere acceso a NuGet.
## Estructura del repositorio
El código de aplicación y las pruebas están en `code/`. La documentación permanece en `docs/` y `specs/`. `infra/` documenta la persistencia local y contiene valores de ejemplo para las variables de entorno; `docker-compose.yml` inicia el entorno base. Más adelante se pueden incorporar archivos de canalización en `pipelines/`.
```text
code/
  ProductInventory.sln
  .editorconfig
  Directory.Build.props
  Directory.Packages.props
  src/
    ProductInventory.Api/
      Dockerfile
  tests/
```
## Configuración compartida de compilación
`code/.editorconfig` define la codificación, los finales de línea, la indentación y el estilo de C# únicamente dentro de `code/`; la documentación de la raíz y las futuras carpetas de infraestructura o canalizaciones quedan fuera de su alcance. Se conservan los espacios finales de Markdown para los saltos de línea explícitos. Las reglas de líneas en blanco y longitud de métodos del ADR-008 todavía requieren revisión manual.
La selección del SDK depende del entorno instalado; actualmente no se incluye global.json. `code/Directory.Build.props` define `net9.0`, los tipos de referencia que admiten valores nulos y las importaciones implícitas para los siete proyectos. `code/Directory.Packages.props` habilita la administración central de paquetes NuGet y contiene todas las versiones de dependencias directas; cada archivo de proyecto declara solo sus propias referencias. Agregar una versión central no incorpora el paquete a ningún proyecto.
## Restauración, compilación y pruebas
Ejecuta los siguientes comandos desde la raíz del repositorio. Si ya estás dentro de `code/`, omite `Set-Location code`.
```powershell
Set-Location code
dotnet restore ProductInventory.sln
dotnet build ProductInventory.sln --no-restore
$env:PRODUCTINVENTORY_TEST_CONNECTION_STRING = 'Server=localhost;Integrated Security=true;Encrypt=true;TrustServerCertificate=true'
dotnet test ProductInventory.sln --no-build --no-restore
```
Hay cuatro proyectos de código fuente y tres de pruebas. Las pruebas de integración requieren SQL Server real y permiso CREATE DATABASE; la ausencia de conexión produce un fallo y no omite las pruebas de base de datos. Cada ejecución crea bases con nombres únicos y las conserva para inspección. No se reinicia ni elimina ninguna base existente. Domain.Tests y Application.Tests todavía no contienen pruebas; los manejadores y el comportamiento completo del dominio corresponden a etapas posteriores. Todos los proyectos de pruebas referencian Moq.
## Ejecución y depuración local
```powershell
Set-Location code
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:5181'
$env:Identity__Authority = 'http://localhost:8081/realms/productinventory'
$env:Identity__MetadataAddress = 'http://localhost:8081/realms/productinventory/.well-known/openid-configuration'
$env:Identity__Audience = 'productinventory-api'
$env:Swagger__Enabled = 'true'
$env:Swagger__ClientId = 'productinventory-swagger'
$env:ConnectionStrings__Inventory = 'Server=localhost;Database=ProductInventory;Integrated Security=true;Encrypt=true;TrustServerCertificate=true'
$env:Database__Initialize = 'true'
dotnet run --project src/ProductInventory.Api --no-launch-profile
Invoke-WebRequest http://127.0.0.1:5181/health
```
Ejecuta el último comando desde otra terminal. La respuesta es `200`, con el texto sin formato `Healthy`. Detén la API con Ctrl+C. Para depurar localmente, abre `code/ProductInventory.sln` en Visual Studio, selecciona ProductInventory.Api como proyecto de inicio, configura ASPNETCORE_URLS en el entorno del depurador y coloca un punto de interrupción en Program.Main.
La configuración de ejecución se lee de variables de entorno. `Database__Initialize=true` crea la base indicada si no existe y aplica el esquema inicial embebido antes de aceptar solicitudes; requiere permisos de creación de bases y DDL. El valor predeterminado es false. Cuando hay una conexión configurada, GET /health comprueba la conectividad SQL y devuelve 503 si no está disponible. Sin conexión configurada, comprueba únicamente la salud del proceso. No detecta diferencias respecto al esquema esperado. Las carpetas de funcionalidades contienen contratos de persistencia y DTO; no se exponen rutas HTTP de negocio.
## Docker y persistencia
Consulta la [configuración de persistencia local](infra/README.md) para conocer Compose, las variables de entorno, la inicialización del esquema y las pruebas de integración. Compose contiene API, SQL Server y Keycloak, con volúmenes y comprobaciones de disponibilidad. Consulta la [guía de seguridad y validación manual](infra/keycloak/README.md) para login, JWT, PKCE y configuración de los emisores. Las pruebas automatizadas existentes solo requieren SQL Server; no necesitan Keycloak ni la API en contenedores.
`InventoryReadDbContext` mapea cuatro tablas y realiza proyecciones de solo lectura con EF Core 9. Dapper crea productos con existencias iniciales en cero. El registro de movimientos actualiza las existencias de forma condicional e inserta historial inmutable IN/OUT en una sola transacción SQL, con fecha UTC y existencias anteriores y posteriores. Los fallos de inserción y la cancelación revierten ambos efectos; las operaciones concurrentes no permiten salidas superiores a las existencias ni pierden actualizaciones.
## Arquitectura y alcance
Domain no tiene dependencias de proyectos ni paquetes. Application referencia Domain; Infrastructure referencia Application y Domain; Api referencia Application e Infrastructure. Los manejadores de casos de uso y los endpoints funcionales aún no se implementan. La etapa 5.3 incorpora JWT Bearer, Keycloak y Swagger PKCE. La API exige autenticación por defecto; GET /health es explícitamente anónimo. No se agregan roles ni endpoints de diagnóstico de seguridad.
Antes de implementar, lee `constraints.md`, `specs/requirements_catalog.md`, `specs/entity_model.md`, los 17 casos de uso y los ADR aceptados en inglés. Sus traducciones al español están en `docs/adr/es/`.
Persisten inconsistencias documentales conocidas: el catálogo indica 200 para movimientos, mientras UC-006 exige 201; la visión y el resumen de especificaciones todavía excluyen los movimientos históricos; varios estados de especificación aún dicen Draft. No afectan a la estructura inicial y requieren revisión antes de implementar el negocio.
## Seguimiento de ejecución
El seguimiento y los resultados TRX se conservan localmente en docs/implementation/. La etapa 5.2 está APPROVED desde el 2026-11-12; la etapa 5.3 está APPROVED desde el 2026-11-12 tras registrar su cierre COMPLETED y la validación manual del responsable; 5.4 está READY sin implementar. La suite automatizada mantiene 29 pruebas y se verificó con Keycloak detenido. La ejecución en contenedores se verificó el 2026-11-12: SQL Server 16.0.4185.3 está saludable, la API responde 200 Healthy en http://localhost:5181/health y las 29 pruebas de integración pasan contra la base del contenedor. La validación anterior con SQL Server local 15.0.2210.4 también fue satisfactoria. El destino seleccionado sigue siendo net9.0; cambiarlo requiere aprobación.
