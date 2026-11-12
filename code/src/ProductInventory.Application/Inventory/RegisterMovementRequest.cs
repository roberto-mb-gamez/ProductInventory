using ProductInventory.Domain.Inventory;
namespace ProductInventory.Application.Inventory;
public sealed record RegisterMovementRequest(Guid ProductId, MovementType Type, int Quantity);
