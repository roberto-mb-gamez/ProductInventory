# Seguridad local y validación manual
La etapa 5.3 integra Keycloak, JWT Bearer y Swagger con Authorization Code + PKCE. Por indicación explícita del responsable del proyecto, la seguridad se valida manualmente ejecutando la aplicación. La suite automatizada no requiere Keycloak ni ejecuta pruebas de autenticación o PKCE.
## Preparar el entorno
Conserva el `.env` de la raíz. Si ya existe, agrega las variables nuevas de `infra/.env.example` sin reemplazar la contraseña SQL anterior:
- `KEYCLOAK_ADMIN_PASSWORD`: contraseña local del administrador de Keycloak.
- `KEYCLOAK_DEVELOPMENT_PASSWORD`: contraseña local del usuario de demostración.
- `KEYCLOAK_PORT`: puerto público; 8081 por defecto.
En este equipo ya se generaron contraseñas aleatorias en el `.env` ignorado por Git. No copies esos valores a archivos versionables.
Desde la raíz:
```powershell
docker compose up --build -d
docker compose ps
```
La API queda en http://localhost:5181, Swagger en http://localhost:5181/swagger/ y Keycloak en http://localhost:8081. Si una terminal antigua no encuentra docker-credential-desktop, reiníciala o aplica el ajuste temporal de PATH explicado en `../README.md`.
Keycloak usa la imagen 26.8.2 fijada por digest, almacenamiento embebido de desarrollo en `keycloak-data` e importa `productinventory-realm.json`. SQL Server sigue siendo la única base de datos de la aplicación. La importación inicial sustituye las contraseñas y las direcciones de Swagger desde variables de entorno; no incluye claves privadas ni credenciales reales en el repositorio.
## Usuario y clientes
| Elemento | Valor |
|---|---|
| Realm | productinventory |
| Usuario de demostración | inventory-developer |
| Contraseña del usuario | Valor local de KEYCLOAK_DEVELOPMENT_PASSWORD |
| Administrador de Keycloak | KEYCLOAK_ADMIN_USERNAME; development-admin por defecto |
| Contraseña del administrador | Valor local de KEYCLOAK_ADMIN_PASSWORD |
| Cliente público de Swagger | productinventory-swagger |
| Audiencia de la API | productinventory-api |
El cliente Swagger utiliza exclusivamente Authorization Code con PKCE S256; no tiene secreto, grant de contraseña ni flujo implícito. Las direcciones de retorno admitidas son las rutas exactas `/swagger/oauth2-redirect.html` en localhost y 127.0.0.1, con el puerto API_PORT. No hay permisos de negocio, roles adicionales ni gestión de usuarios dentro de la API.
## Validar Swagger y login
1. Abre http://localhost:5181/swagger/ y pulsa **Authorize**.
2. Selecciona el esquema OAuth2 y los ámbitos `openid` y `profile`. El cliente debe ser `productinventory-swagger`; no proporciones secreto de cliente.
3. Autoriza e inicia sesión en Keycloak con `inventory-developer` y la contraseña local del usuario.
4. Confirma que regresas a Swagger con la autorización completada.
5. En las herramientas de red del navegador, verifica que la solicitud de autorización incluye `response_type=code`, `code_challenge_method=S256` y `code_challenge`; el intercambio debe usar `grant_type=authorization_code` y `code_verifier`, sin `client_secret`.
6. Comprueba que el access token contiene `iss=http://localhost:8081/realms/productinventory`, audiencia `productinventory-api`, firma RS256 y `exp`. Usa el access token, no el ID token ni el refresh token. No guardes los tokens en documentación, capturas compartidas ni archivos versionables.
Todavía no hay endpoints CRUD. Swagger muestra el esquema de autenticación y permite completar el login aunque aún no existan operaciones de negocio para probar.
## Comprobar acceso HTTP
Desde otra terminal:
```powershell
Invoke-WebRequest http://localhost:5181/health
Invoke-WebRequest http://localhost:5181/api/products -SkipHttpErrorCheck
```
La salud es explícitamente anónima: debe devolver `200 Healthy`. La segunda solicitud debe devolver `401` sin token, por la política global de autenticación.
Para una comprobación manual con el access token obtenido del navegador:
```powershell
$accessToken = Read-Host -MaskInput 'Access token'
$headers = @{ Authorization = "Bearer $accessToken" }
Invoke-WebRequest http://localhost:5181/api/products -Headers $headers -SkipHttpErrorCheck
Remove-Variable accessToken, headers
```
Con un access token válido se espera `404`: la autenticación pasa y la ruta CRUD todavía no está implementada. Ese resultado no implica que exista un caso de uso de productos. No se agrega un endpoint de diagnóstico para validar seguridad.
## Casos negativos manuales
| Caso | Preparación | Resultado esperado |
|---|---|---|
| Token ausente | Omitir Authorization | 401 |
| Token malformado | Usar Bearer invalid-token | 401 |
| Firma alterada | Modificar un carácter del segmento de firma del access token | 401 |
| Audiencia incorrecta | Usar el ID token legítimo emitido al cliente Swagger, cuya audiencia es productinventory-swagger | 401 |
| Token expirado | Conservar el access token y esperar hasta después de exp más 30 segundos de tolerancia | 401 |
| Emisor incorrecto | Usar un token legítimo de otro emisor; no desactivar validadores ni agregar ese emisor a configuración | 401 |
| PKCE ausente o incorrecto | Repetir la autorización sin challenge o intercambiar el código con un verifier diferente usando las herramientas HTTP del navegador | Keycloak rechaza la autorización o el intercambio |
Modificar `iss` o `aud` en el contenido de un token sin volver a firmarlo también invalida su firma; ese caso no demuestra por separado la validación de emisor o audiencia. Registra el caso realmente ejecutado y no marques el resto como probado.
## Emisor público y discovery interno
La API valida siempre el emisor público `Identity__Authority=http://localhost:8081/realms/productinventory`. En Compose, `Identity__MetadataAddress` apunta a `http://keycloak:8080/realms/productinventory/.well-known/openid-configuration`. Keycloak tiene hostname público fijo y backchannel dinámico: mantiene el emisor público, pero publica un JWKS accesible por la dirección interna cuando la API consulta discovery. Swagger utiliza los endpoints públicos para que el navegador pueda resolverlos.
En depuración local con Visual Studio, usa Development y estas variables del proceso de la API:
```text
ASPNETCORE_ENVIRONMENT=Development
Identity__Authority=http://localhost:8081/realms/productinventory
Identity__MetadataAddress=http://localhost:8081/realms/productinventory/.well-known/openid-configuration
Identity__Audience=productinventory-api
Swagger__Enabled=true
Swagger__ClientId=productinventory-swagger
```
La API requiere firma RS256, emisor y audiencia correctos, expiración y vigencia válidas; no se sustituyen validadores ni se aceptan tokens sin firma. La tolerancia de reloj es de 30 segundos. Fuera de Development, las direcciones de identidad requieren HTTPS y las solicitudes HTTP a la API se rechazan; configura TLS antes de usar otro entorno. La configuración local HTTP y el modo start-dev de Keycloak no son de producción.
## Persistencia del realm
El volumen `keycloak-data` conserva el realm entre reinicios. `--import-realm` omite un realm ya existente: cambiar el JSON o la contraseña del `.env` no modifica automáticamente un realm importado. No borres el volumen para aplicar cambios sin revisar antes los datos; una reinicialización destructiva requiere autorización explícita.
## Pruebas automatizadas sin Keycloak
Para ejecutar la suite existente solo necesitas SQL Server, con la misma configuración de integración de 5.2. Desde la raíz puedes iniciar únicamente SQL:
```powershell
docker compose up -d sqlserver
```
Después ejecuta `dotnet test ProductInventory.sln` desde `code/`. No hace falta iniciar la API ni Keycloak. Las pruebas de health usan una configuración local sin proveedor de identidad accesible y no envían tokens. La suite de 29 pruebas se verificó con Keycloak detenido; no contiene pruebas de JWT, login, PKCE ni dependencias de Keycloak en `.runsettings`.
## Registro de validación manual
La implementación y el arranque están comprobados. El 2026-11-12, el responsable del proyecto confirmó que la validación manual funcionó al 100%. Esta confirmación se registró como evidencia humana; después se cerró 5.3 como COMPLETED y se aplicó su aprobación explícita, quedando APPROVED. No se atribuye la ejecución manual a Codex. Los intentos de pruebas automatizadas retiradas permanecen como historial. La etapa 5.4 está READY y todavía no está autorizada para implementación.
