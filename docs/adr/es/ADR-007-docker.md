# ADR-007 — Entorno reproducible con Docker Compose
**Estado:** Aceptado

## Decisión
- Proporcionar un Dockerfile multietapa para la API .NET 9 y `docker-compose.yml` con `productinventory-api`, `sqlserver` y `keycloak`.
- `docker compose up --build -d` debe iniciar el entorno desde una copia limpia del repositorio, con configuración de desarrollo documentada y la inicialización necesaria de base de datos y realm.
- Persistir los datos de SQL Server en un volumen con nombre. Utilizar health checks y reintentos para evitar que la API arranque antes de que sus dependencias estén disponibles. Configurar variables de entorno y secretos de Compose cuando corresponda; no publicar secretos reales.
- SQL Server será la única base de datos de la aplicación. Keycloak podrá utilizar su almacenamiento integrado en modo desarrollo para esta prueba; no presentarlo como configuración productiva.
- Documentar puertos, comprobaciones de salud, inicialización del esquema SQL, importación de Keycloak y depuración local.

## Consecuencias
Es necesario comprobar el arranque real de Compose, no solo documentarlo. Keycloak y SQL Server pueden requerir bastante memoria local.
