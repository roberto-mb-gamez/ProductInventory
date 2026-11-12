# ADR-003 — Domain boundaries and transactional inventory movements
Status: Accepted

## Context
The assessment requires Products, Categories and InventoryMovements tables. Approved MVP also includes Suppliers and 17 use cases.

## Decision
- Model Product, Category, Supplier, StockMovement. Product stock starts at zero; only UC-006 changes it.
- UC-006 registers an immutable `IN` or `OUT` movement (positive quantity), records UTC timestamp and before/after stock, and updates Product stock in the **same SQL transaction**.
- `dbo.InventoryMovement_Register`, invoked through Dapper, owns that transaction with `SET XACT_ABORT ON` and TRY/CATCH rollback. It performs the conditional stock update and immutable history insert, records UTC in SQL Server, and returns the movement after commit. The writer supplies a new movement identifier and maps explicit outcomes without duplicating transaction ownership. Application/Domain input validation remains in place; SQL constraints and procedure guards protect direct reuse.
- Protect against concurrent updates using a conditional atomic stock update or equivalent concurrency-safe SQL strategy; roll back if the movement insert fails. An insufficient-stock exit is rejected without side effects.
- Product with movement history cannot be deleted; Category/Supplier with products cannot be deleted. No cascading inventory-history deletion.
- SQL uniqueness and foreign keys enforce invariants in addition to Application validation. Use an appropriate case-insensitive uniqueness policy for SKU and category name.
- Do not create Inventory as a separate domain entity for this MVP; Product holds current stock, StockMovement records history.

## Consequences
Inventory correctness takes precedence over generic CRUD patterns. Exact schema, migration and locking details are implementation choices subject to tests.
