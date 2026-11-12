namespace ProductInventory.Application.Products;
public sealed record ProductDto(Guid Id, string Sku, string Name, string? Description, decimal Price, int StockQuantity, Guid CategoryId, Guid SupplierId);
