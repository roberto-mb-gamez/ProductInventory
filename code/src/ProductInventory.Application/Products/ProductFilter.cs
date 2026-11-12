namespace ProductInventory.Application.Products;
public sealed record ProductFilter(Guid? CategoryId = null, Guid? SupplierId = null, int? MaxStock = null);
