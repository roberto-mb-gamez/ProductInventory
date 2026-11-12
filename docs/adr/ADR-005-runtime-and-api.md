# ADR-005 — .NET 9 and ASP.NET Core Minimal APIs
Status: Accepted (user-selected for local tooling)

## Decision
- Target `net9.0`, use ASP.NET Core 9 Minimal APIs, EF Core 9, SQL Server provider compatible with EF Core 9, and compatible Dapper version.
- Organize endpoint mapping by feature and keep business logic in Application.
- Use OpenAPI with Swagger UI, including a documented OAuth2 security scheme.
- Keep runtime configuration in environment variables; `.env.example` may contain placeholders only.

## Support risk
.NET 9 reached end of support on 2026-11-10. The user selected it because of Visual Studio 2022 compatibility. This is a conscious exception for the technical exercise, not a recommendation for new production systems. Prefer upgrading to .NET 10 LTS when the development environment permits; do not change target without approval.
