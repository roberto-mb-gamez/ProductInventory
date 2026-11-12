namespace ProductInventory.Domain.Inventory;
public sealed record MovementData(Guid ProductId, MovementType Type, int Quantity);
