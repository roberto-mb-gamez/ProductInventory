# ADR-006 — OAuth2/OIDC con Keycloak y validación JWT Bearer
**Estado:** Aceptado

## Decisión
- Keycloak será el servidor externo de autorización/proveedor de identidad. ProductInventory no implementará registro de usuarios, almacenamiento de contraseñas ni endpoints propios de inicio de sesión.
- La API validará access tokens JWT firmados mediante middleware JWT Bearer de ASP.NET Core: firma, emisor, audiencia prevista y vigencia. Exigir HTTPS fuera del desarrollo local.
- Todos los endpoints de negocio requieren autenticación. Un token ausente, inválido o expirado devuelve `401`; un token válido sin autorización requerida, si posteriormente se introduce una política, devuelve `403`.
- Swagger UI utilizará Authorization Code con PKCE y un cliente público con la URI de redirección correcta. Configurar un recurso/audiencia de API separado para que el token esté destinado a ProductInventory.
- Compose podrá importar un realm y clientes de Keycloak **solo para desarrollo**, sin credenciales productivas. El cliente público de Swagger, ejecutado en el navegador, no debe tener secreto.
- Preferir descubrimiento OIDC/JWKS frente a claves de firma codificadas. Asegurar que la URL del emisor funcione coherentemente para la API en contenedor y los clientes del navegador. Verificar emisor y audiencia con el realm realmente importado.

## Consecuencias
El entorno requiere un tercer contenedor. La administración de permisos y una matriz RBAC quedan fuera del alcance del MVP.
