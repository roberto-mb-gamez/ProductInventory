namespace ProductInventory.Application.Products;
public interface IProductReader
{
    Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
