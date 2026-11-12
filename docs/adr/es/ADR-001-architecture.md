# ADR-001 — Clean Architecture ligera
**Estado:** Aceptado

## Contexto
ProductInventory es una prueba técnica pequeña para una API RESTful de inventario. El libro *Clean Architecture with .NET* (Crouse y Smith, Packt, 2026), especialmente sus capítulos 2 a 5, fundamenta las dependencias dirigidas hacia el núcleo y advierte sobre el exceso de estructura en aplicaciones centradas en CRUD.

## Decisión
Construir un monolito desplegable con cuatro proyectos: `ProductInventory.Domain`, `ProductInventory.Application`, `ProductInventory.Infrastructure` y `ProductInventory.Api`.
- **Domain:** entidades, invariantes y reglas de dominio; sin dependencias de ASP.NET, EF Core, Dapper ni SQL Server.
- **Application:** comandos, consultas, handlers, DTO y contratos de persistencia; referencia Domain.
- **Infrastructure:** implementaciones de contratos de Application, modelo de lectura con EF Core, escrituras con Dapper y configuración de SQL Server; referencia Application y Domain.
- **Api:** endpoints HTTP, autenticación JWT Bearer, OpenAPI/Swagger y punto de composición de inyección de dependencias; referencia Application e Infrastructure.
- Las referencias entre proyectos nunca deben crear dependencias del núcleo hacia capas externas. La API no puede omitir Application para acceder directamente a persistencia.

## Alternativas descartadas
- API CRUD de un solo proyecto: menor separación de responsabilidades.
- Arquitectura empresarial completa: complejidad excesiva para la prueba.

## Consecuencias
Límites explícitos, pruebas unitarias sencillas y cantidad moderada de proyectos. No se requieren microservicios, bus de mensajes, repositorio genérico, Event Sourcing ni un proyecto adicional `Common`.
