namespace ProductInventory.Domain.Products;
public sealed record ProductData(string Sku, string Name, string? Description, decimal Price, Guid CategoryId, Guid SupplierId);
