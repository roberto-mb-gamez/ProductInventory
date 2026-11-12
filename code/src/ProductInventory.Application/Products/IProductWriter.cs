namespace ProductInventory.Application.Products;
public interface IProductWriter
{
    Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
}
