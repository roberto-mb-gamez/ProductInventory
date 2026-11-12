namespace ProductInventory.Application.Inventory;
public sealed record MovementWriteResult(MovementWriteStatus Status, StockMovementDto? Movement = null);
