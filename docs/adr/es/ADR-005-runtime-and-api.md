# ADR-005 — .NET 9 y ASP.NET Core Minimal APIs
**Estado:** Aceptado (selección del usuario por compatibilidad con herramientas locales)

## Decisión
- Utilizar `net9.0`, ASP.NET Core 9 Minimal APIs, EF Core 9, un proveedor de SQL Server compatible con EF Core 9 y una versión compatible de Dapper.
- Organizar el mapeo de endpoints por funcionalidad y mantener la lógica de negocio en Application.
- Publicar OpenAPI con Swagger UI e incluir un esquema de seguridad OAuth2 documentado.
- Leer la configuración de ejecución desde variables de entorno. `.env.example` solo puede contener valores de ejemplo o marcadores de posición.

## Riesgo de soporte
.NET 9 terminó su soporte el 10 de noviembre de 2026. Se eligió por compatibilidad con Visual Studio 2022. Es una excepción consciente para la prueba técnica, no una recomendación para nuevos sistemas productivos. Se recomienda migrar a .NET 10 LTS cuando el entorno lo permita; no cambiar el framework sin aprobación.
