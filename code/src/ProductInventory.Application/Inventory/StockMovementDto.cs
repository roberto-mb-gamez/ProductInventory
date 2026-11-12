using ProductInventory.Domain.Inventory;
namespace ProductInventory.Application.Inventory;
public sealed record StockMovementDto(Guid Id, Guid ProductId, MovementType Type, int Quantity, DateTimeOffset OccurredAt, int StockBefore, int StockAfter);
