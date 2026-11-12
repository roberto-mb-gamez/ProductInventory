# Product Vision & Scope — ProductInventory

**Estado:** Aprobado el alcance general; detalles de negocio propuestos para validación.

## Visión
API RESTful para gestionar un catálogo de productos con categoría, proveedor principal y existencias disponibles, de manera consistente y con acceso protegido mediante OAuth2.

## Objetivos
- Registrar y mantener productos, categorías y proveedores.
- Consultar productos junto con sus datos relacionados y aplicar filtros.
- Registrar movimientos de inventario sin permitir valores negativos.
- Rechazar acceso anónimo a operaciones protegidas.

## Actor
**Administrador de Inventario:** persona o sistema cliente que consume la API con un token válido. No implica gestión de usuarios o roles en este producto.

## Incluido
- Productos: crear, consultar, listar, actualizar y eliminar.
- Categorías: crear, consultar, listar, actualizar y eliminar.
- Proveedores: crear, consultar, listar, actualizar y eliminar.
- Inventario: ajustar unidades de un producto.
- Seguridad transversal: validar acceso mediante token OAuth2.
- Filtros básicos de productos por categoría, proveedor y stock máximo; paginación.

## Excluido
- Registro, login propio, logout, recuperación de contraseñas, roles y administración de usuarios.
- Movimientos históricos, auditoría funcional, reservas, compras, ventas y almacenes múltiples.
- Gestión de múltiples proveedores por producto, categorías jerárquicas, borrado lógico.
- Reportes analíticos, notificaciones y exportaciones.

## Criterio de éxito
Los 17 casos de uso tienen resultados observables, reglas de negocio y criterios de aceptación trazables; el conjunto puede implementarse sin inventar funcionalidad adicional.

## Pendiente de etapa 4
Elección de stack, proveedor de identidad, flujo OAuth2, configuración de tokens, persistencia, contenedores y decisiones arquitectónicas.
