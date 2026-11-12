namespace ProductInventory.Infrastructure.Persistence.ReadModels;
public sealed class StockMovementRow
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public int StockBefore { get; set; }
    public int StockAfter { get; set; }
}
