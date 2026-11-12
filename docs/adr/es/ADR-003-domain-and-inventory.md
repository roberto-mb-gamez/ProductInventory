# ADR-003 — Límites del dominio y movimientos de inventario transaccionales
**Estado:** Aceptado

## Contexto
La prueba requiere tablas Products, Categories e InventoryMovements. El MVP aprobado también incorpora Suppliers y 17 casos de uso.

## Decisión
- Modelar Product, Category, Supplier y StockMovement. Las existencias de Product comienzan en cero y únicamente UC-006 puede modificarlas.
- UC-006 registra un movimiento inmutable de tipo `IN` o `OUT`, con cantidad positiva, fecha UTC y existencias anteriores y posteriores; actualiza las existencias del producto en la **misma transacción SQL**.
- `dbo.InventoryMovement_Register`, invocado mediante Dapper, controla esa transacción con `SET XACT_ABORT ON` y rollback en TRY/CATCH. Realiza la actualización condicional del stock y la inserción inmutable del historial, registra UTC en SQL Server y devuelve el movimiento después del commit. El writer proporciona un identificador nuevo y traduce resultados explícitos sin duplicar el control de la transacción. Se conserva la validación de entrada de Application/Domain; las restricciones SQL y las guardas del procedimiento protegen la reutilización directa.
- Proteger las actualizaciones concurrentes mediante una actualización atómica condicional del stock o una estrategia SQL equivalente. Revertir la transacción si falla la inserción del movimiento. Rechazar salidas con stock insuficiente sin efectos secundarios.
- No eliminar productos con movimientos históricos ni categorías/proveedores que tengan productos asociados. No eliminar historial en cascada.
- Aplicar restricciones de unicidad y claves foráneas en SQL, además de las validaciones de Application. Utilizar una política de unicidad adecuada sin distinción de mayúsculas/minúsculas para SKU y nombre de categoría.
- No crear una entidad de dominio Inventory adicional en este MVP: Product conserva las existencias actuales y StockMovement registra el historial.

## Consecuencias
La integridad del inventario prevalece sobre los patrones CRUD genéricos. Los detalles exactos de esquema, migración y bloqueo se decidirán durante la implementación y se comprobarán mediante pruebas.
