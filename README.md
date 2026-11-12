# ProductInventory — Stage 5.1 bootstrap
The repository contains the approved specifications and a minimal .NET 9 bootstrap. Stage 5.1 is APPROVED by the user; later stages have not started.
## Requirements
Install a .NET SDK capable of targeting net9.0 and the ASP.NET Core 9 runtime. The bootstrap was verified with SDK 9.0.321. NuGet access is required on the first restore.
## Repository layout
All application and test code lives under `code/`. Documentation stays in `docs/` and `specs/`; root-level `infra/` and `pipelines/` may be added in later authorized stages.
```text
code/
  ProductInventory.sln
  .editorconfig
  Directory.Build.props
  Directory.Packages.props
  src/
  tests/
```
## Shared build configuration
`code/.editorconfig` defines encoding, line endings, indentation and C# style only inside `code/`; root documentation and future infrastructure or pipeline folders are outside its scope. Markdown trailing spaces are preserved for explicit line breaks. ADR-008 blank-line and method-length rules still require manual review.
SDK selection uses the installed environment; no global.json is currently included. `code/Directory.Build.props` defines `net9.0`, nullable reference types and implicit usings for all seven projects. `code/Directory.Packages.props` enables NuGet Central Package Management and holds every direct package version; project files list only their own package references. Adding a central version does not add the package to any project.
## Restore, build and test
Run the following commands from the repository root. If already inside `code/`, omit `Set-Location code`.
```powershell
Set-Location code
dotnet restore ProductInventory.sln
dotnet build ProductInventory.sln --no-restore
dotnet test ProductInventory.sln --no-build --no-restore
```
There are four source projects and three test projects. The health integration test uses xUnit and WebApplicationFactory. Domain.Tests and Application.Tests currently contain no tests; domain behavior belongs to later authorized stages. All test projects reference Moq.
## Run and debug locally
```powershell
Set-Location code
$env:ASPNETCORE_URLS = 'http://127.0.0.1:5181'
dotnet run --project src/ProductInventory.Api --no-launch-profile
Invoke-WebRequest http://127.0.0.1:5181/health
```
Run the last command from another terminal. The response is `200` with plain text `Healthy`. Stop the API with Ctrl+C. For local debugging, open `code/ProductInventory.sln` in Visual Studio, select ProductInventory.Api as the startup project, configure ASPNETCORE_URLS in the debugger environment and set a breakpoint in Program.Main.
Runtime configuration is read from environment variables. The bootstrap exposes only GET /health, which checks process health without external dependencies. It does not verify database readiness. Feature folders are reserved under Api and Application without use-case implementations.
## Architecture and scope
Domain has no project or package dependencies. Application references Domain; Infrastructure references Application and Domain; Api references Application and Infrastructure. No business logic, persistence, Keycloak, Swagger or Docker has been implemented in this stage.
Read `constraints.md`, `specs/requirements_catalog.md`, `specs/entity_model.md`, the 17 use cases and accepted English ADRs before implementation. Spanish ADR translations remain under `docs/adr/es/`.
Known documentation inconsistencies remain unresolved: the catalog states 200 for movements while UC-006 requires 201; vision and the specs summary still exclude historical movements; several specification statuses still say Draft. These do not affect bootstrap and require review before business implementation.
## Execution tracking
Execution tracking is maintained locally under docs/implementation/ and is excluded from this commit. Deployment and external service setup belong to later stages and are not available yet. The selected target remains net9.0; changing it requires approval.
