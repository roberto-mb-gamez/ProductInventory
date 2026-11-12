namespace ProductInventory.Domain.Inventory;
public sealed class StockConflictException(string message) : Exception(message);
