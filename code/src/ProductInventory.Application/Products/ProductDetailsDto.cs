namespace ProductInventory.Application.Products;
public sealed record ProductDetailsDto(Guid Id, string Sku, string Name, string? Description, decimal Price, int StockQuantity, Guid CategoryId, string CategoryName, Guid SupplierId, string SupplierName);
