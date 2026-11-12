# ProductInventory — API de inventario con .NET 9
Las etapas 5.1, 5.2 y 5.3 están APPROVED. La etapa 5.4 está COMPLETED, pendiente de aprobación humana: 17/17 casos de uso cubiertos, CRUD de categorías, proveedores y productos, y movimientos IN/OUT. Restore, build y 107 pruebas pasaron; Compose inicia y /health devuelve 200 Healthy.
## Requisitos
Para compilar y ejecutar la API localmente necesitas:

- **.NET 9 SDK** para tu sistema operativo. Incluye el runtime de .NET y ASP.NET Core necesarios para ejecutar la API. Descárgalo desde la [página oficial de .NET 9](https://dotnet.microsoft.com/download/dotnet/9.0); instala el SDK, no solo el runtime. El proyecto apunta a `net9.0` y no fija una versión puntual del SDK.
- **Git**, para clonar el repositorio, y **acceso a Internet** durante `dotnet restore` para descargar paquetes de NuGet.

Para trabajar desde Visual Studio en Windows, instala **Visual Studio 2022 17.12 o posterior** con la carga de trabajo **ASP.NET y desarrollo web**. También puedes compilar y ejecutar desde una terminal con el SDK, sin Visual Studio.

Para iniciar los servicios locales y ejecutar las pruebas de integración necesitas **Docker Desktop** con Docker Engine y Docker Compose v2 disponibles. Compose ya viene incluido en Docker Desktop; no hace falta instalar Docker Compose por separado. Descarga Docker Desktop desde su [página oficial](https://www.docker.com/products/docker-desktop/) y consulta la [guía oficial de instalación de Compose](https://docs.docker.com/compose/install/). El entorno Compose levanta SQL Server, Keycloak y la API. Las pruebas de integración usan SQL Server real y requieren permiso `CREATE DATABASE`.

Comprueba la instalación con `dotnet --list-sdks`, `docker --version` y `docker compose version`. La primera restauración requiere acceso a NuGet.
## Estructura del repositorio
El repositorio separa el código ejecutable, la documentación del proyecto y los recursos para el entorno local. La solución y sus siete proyectos viven en `code/`; así, otros recursos de despliegue o automatización pueden agregarse en carpetas raíz independientes.

```text
ProductInventory/
├── code/
│   ├── ProductInventory.sln
│   ├── Directory.Build.props
│   ├── Directory.Packages.props
│   ├── .editorconfig
│   ├── src/
│   │   ├── ProductInventory.Api/
│   │   ├── ProductInventory.Application/
│   │   ├── ProductInventory.Domain/
│   │   └── ProductInventory.Infrastructure/
│   └── tests/
│       ├── ProductInventory.Domain.Tests/
│       ├── ProductInventory.Application.Tests/
│       └── ProductInventory.IntegrationTests/
├── docs/
│   └── adr/
├── infra/
│   └── keycloak/
├── specs/
│   └── usecases/
├── docker-compose.yml
└── README.md
```

- **`code/src/ProductInventory.Api/`**: aplicación ASP.NET Core. `Features/` mapea rutas HTTP y enlaza solicitudes con handlers; `Security/` configura JWT Bearer; `Documentation/` configura OpenAPI y Swagger; `Hosting/` registra dependencias y configura el host. Aquí también está el Dockerfile de la API.
- **`code/src/ProductInventory.Application/`**: casos de uso CQRS manuales. Contiene comandos, consultas, handlers, DTOs y puertos de persistencia. Depende de Domain y no contiene acceso directo a SQL ni referencias a Infrastructure.
- **`code/src/ProductInventory.Domain/`**: entidades, reglas e invariantes de negocio. Se mantiene independiente de ASP.NET Core, EF Core, Dapper y SQL Server.
- **`code/src/ProductInventory.Infrastructure/`**: adaptadores de persistencia. Implementa las lecturas con EF Core y las escrituras con Dapper; contiene el acceso a SQL Server y las implementaciones de los puertos de Application.
- **`code/tests/ProductInventory.Domain.Tests/`**: pruebas unitarias de entidades e invariantes del dominio.
- **`code/tests/ProductInventory.Application.Tests/`**: pruebas unitarias de handlers, validaciones y puertos de aplicación.
- **`code/tests/ProductInventory.IntegrationTests/`**: pruebas HTTP y de persistencia contra SQL Server real; no usa una base de datos en memoria.
- **`code/Directory.Build.props` y `code/Directory.Packages.props`**: centralizan propiedades de compilación, framework destino y versiones de paquetes NuGet. **`code/.editorconfig`** aplica convenciones de formato al árbol `code/`.
- **`infra/` y `docker-compose.yml`**: configuración y guías para el entorno local. `infra/keycloak/` guarda la configuración del realm de desarrollo. Los secretos se proporcionan por variables de entorno o `.env` local ignorado por Git.
- **`specs/`**: requisitos, modelo de entidades, trazabilidad y casos de uso aprobados. **`docs/adr/`** contiene las decisiones arquitectónicas aceptadas y sus traducciones.
- **`pipelines/`**: carpeta prevista para automatización CI/CD futura; todavía no forma parte de la estructura actual.

Las dependencias entre proyectos siguen esta dirección: `Domain` no depende de otros proyectos; `Application` depende de `Domain`; `Infrastructure` implementa los puertos de `Application`; y `Api` compone las capas externas.
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
Hay cuatro proyectos de código fuente y tres de pruebas. Las pruebas de integración requieren SQL Server real y permiso CREATE DATABASE; la ausencia de conexión produce un fallo y no omite las pruebas de base de datos. Cada ejecución crea bases con nombres únicos y las conserva para inspección. No se reinicia ni elimina ninguna base existente. La suite contiene 20 pruebas de Domain, 23 de Application y 64 de integración. Los tests HTTP usan una identidad exclusiva del host de pruebas para comprobar negocio; no validan JWT ni necesitan Keycloak. La seguridad se revisa manualmente. Todos los proyectos de pruebas referencian Moq.
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
La configuración de ejecución se lee de variables de entorno. `Database__Initialize=true` crea la base indicada si no existe y aplica el esquema inicial embebido antes de aceptar solicitudes; requiere permisos de creación de bases y DDL. El valor predeterminado es false. Cuando hay una conexión configurada, GET /health comprueba la conectividad SQL y devuelve 503 si no está disponible. Sin conexión configurada, comprueba únicamente la salud del proceso. No detecta diferencias respecto al esquema esperado. Las rutas HTTP de negocio requieren un token válido.
## Docker y persistencia
Consulta la [configuración de persistencia local](infra/README.md) para conocer Compose, las variables de entorno, la inicialización del esquema y las pruebas de integración. Compose contiene API, SQL Server y Keycloak, con volúmenes y comprobaciones de disponibilidad. Consulta la [guía de seguridad y validación manual](infra/keycloak/README.md) para login, JWT, PKCE y configuración de los emisores. Las pruebas automatizadas existentes solo requieren SQL Server; no necesitan Keycloak ni la API en contenedores.
`InventoryReadDbContext` mapea cuatro tablas y realiza proyecciones de solo lectura con EF Core 9. Dapper crea productos con existencias iniciales en cero. El registro de movimientos actualiza las existencias de forma condicional e inserta historial inmutable IN/OUT en una sola transacción SQL, con fecha UTC y existencias anteriores y posteriores. Los fallos de inserción y la cancelación revierten ambos efectos; las operaciones concurrentes no permiten salidas superiores a las existencias ni pierden actualizaciones.
## Arquitectura y alcance
Domain no tiene dependencias de proyectos ni paquetes. Application referencia Domain; Infrastructure referencia Application y Domain; Api referencia Application e Infrastructure. Los manejadores explícitos de comandos y consultas implementan los casos de uso sin mediador. La etapa 5.3 incorpora JWT Bearer, Keycloak y Swagger PKCE. La API exige autenticación por defecto; GET /health es explícitamente anónimo. No se agregan roles ni endpoints de diagnóstico de seguridad.
Antes de implementar, lee `constraints.md`, `specs/requirements_catalog.md`, `specs/entity_model.md`, los 17 casos de uso y los ADR aceptados en inglés. Sus traducciones al español están en `docs/adr/es/`.
Los contratos aprobados establecen 201 para movimientos y paginación para los tres listados. La exclusión de movimientos históricos en documentos introductorios no elimina el historial inmutable exigido por UC-006; no se exponen operaciones de consulta, edición ni eliminación de movimientos.
## Seguimiento de ejecución
El seguimiento y los resultados TRX se conservan localmente en docs/implementation/. 5.4 está COMPLETED, sin aprobación automática; 5.5 no se inició. No se realizaron commit, push ni merge. El destino sigue siendo net9.0; cambiarlo requiere aprobación.

## Contratos HTTP
Las rutas /api/categories, /api/suppliers y /api/products ofrecen POST y GET de listado; sus rutas /{id} ofrecen GET, PUT y DELETE. Creación devuelve 201; consulta y actualización 200; borrado 204. Validación devuelve 400, recurso inexistente 404 y conflicto de unicidad o referencias 409.
Los listados reciben page (predeterminado 1) y pageSize (predeterminado 20, rango 1..100), y devuelven items, page, pageSize y totalCount. Categorías y proveedores se ordenan por Name/Id; productos por Sku/Id. Productos acepta filtros combinados categoryId, supplierId y maxStock.
POST /api/products/{id}/movements recibe {"type":"IN","quantity":5} o tipo OUT, devuelve 201 con identificador, fecha UTC y stock anterior/posterior. quantity debe ser positiva. El stock inicial es cero y solo cambia mediante movimientos. Una salida insuficiente devuelve 409 sin efectos parciales.
