# ADR-007 — Reproducible Docker Compose environment
Status: Accepted

## Decision
- Provide a multi-stage Dockerfile for the .NET 9 API and `docker-compose.yml` for `productinventory-api`, `sqlserver`, `keycloak`.
- Keep Compose files, the Visual Studio Compose project and the local `.env` in `code/`, beside the solution. Use `code/.env.example` as the template and run Compose commands from `code/`. Keep the project name `productinventory` stable to reuse named volumes.
- `docker compose up --build -d` starts the environment from a clean checkout with documented development configuration and any required initial database/realm setup.
- SQL Server data uses a named volume; readiness checks/retries prevent premature API startup. Configure environment variables and Compose secrets where appropriate; do not commit real secrets.
- SQL Server is the application's only database. Keycloak may use its development-mode embedded storage for this assessment; do not claim this is a production configuration.
- Document port mappings, health checks, SQL schema initialization, Keycloak import, and local debugging.

## Consequences
Compose startup must be validated, not just described. Keycloak and SQL Server may require substantial local memory.
