namespace ProductInventory.Application.Inventory;
public sealed record RegisterStockMovementCommand(Guid ProductId, StockMovementInput Data);
