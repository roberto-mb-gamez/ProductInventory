# ADR-007 — Reproducible Docker Compose environment
Status: Accepted

## Decision
- Provide a multi-stage Dockerfile for the .NET 9 API and `docker-compose.yml` for `productinventory-api`, `sqlserver`, `keycloak`.
- `docker compose up --build -d` starts the environment from a clean checkout with documented development configuration and any required initial database/realm setup.
- SQL Server data uses a named volume; readiness checks/retries prevent premature API startup. Configure environment variables and Compose secrets where appropriate; do not commit real secrets.
- SQL Server is the application's only database. Keycloak may use its development-mode embedded storage for this assessment; do not claim this is a production configuration.
- Document port mappings, health checks, SQL schema initialization, Keycloak import, and local debugging.

## Consequences
Compose startup must be validated, not just described. Keycloak and SQL Server may require substantial local memory.
