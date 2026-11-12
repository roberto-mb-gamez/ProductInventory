namespace ProductInventory.Application.Products;
public sealed record UpdateProductCommand(Guid Id, CreateProductRequest Data);
