namespace ProductInventory.Application.Inventory;
public enum MovementWriteStatus
{
    Created,
    ProductNotFound,
    InsufficientStock,
    StockLimitExceeded,
    ConcurrencyConflict
}
