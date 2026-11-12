using ProductInventory.Domain.Products;
namespace ProductInventory.Application.Products;
public interface IProductWriter
{
    Task<ProductDto> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<ProductDto?> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
