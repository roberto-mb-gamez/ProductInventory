using ProductInventory.Application.Inventory;
namespace ProductInventory.Api.Features.Movements;
public sealed record StockMovementResponse(Guid Id, Guid ProductId, string Type, int Quantity, DateTimeOffset OccurredAt, int StockBefore, int StockAfter)
{
    public static StockMovementResponse From(StockMovementDto movement)
    {

        return new(movement.Id, movement.ProductId, movement.Type.ToString().ToUpperInvariant(), movement.Quantity,
            movement.OccurredAt, movement.StockBefore, movement.StockAfter);
    }
}
