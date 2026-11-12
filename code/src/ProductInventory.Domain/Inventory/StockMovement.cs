namespace ProductInventory.Domain.Inventory;
public sealed class StockMovement(MovementData data, int stockBefore)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid ProductId { get; } = data.ProductId;
    public MovementType Type { get; } = data.Type;
    public int Quantity { get; } = data.Quantity;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    public int StockBefore { get; } = stockBefore;
    public int StockAfter { get; } = Calculate(data, stockBefore);
    public static void Validate(MovementData data)
    {
        if (!Enum.IsDefined(data.Type) || data.Quantity <= 0)
            throw new ValidationException("El movimiento requiere tipo IN u OUT y cantidad entera positiva.");
    }
    private static int Calculate(MovementData data, int stockBefore)
    {
        Validate(data);
        var delta = data.Type == MovementType.In ? (long)data.Quantity : -(long)data.Quantity;
        var stockAfter = stockBefore + delta;
        if (stockBefore < 0 || stockAfter is < 0 or > int.MaxValue)
            throw new StockConflictException("El movimiento supera las existencias o el límite permitido.");

        return (int)stockAfter;
    }
}
