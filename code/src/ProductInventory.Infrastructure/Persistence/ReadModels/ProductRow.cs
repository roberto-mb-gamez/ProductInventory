namespace ProductInventory.Infrastructure.Persistence.ReadModels;
public sealed class ProductRow
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public Guid SupplierId { get; set; }
    public CategoryRow Category { get; set; } = null!;
    public SupplierRow Supplier { get; set; } = null!;
}
