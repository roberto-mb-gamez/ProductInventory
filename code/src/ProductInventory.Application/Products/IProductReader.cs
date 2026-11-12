using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Products;
public interface IProductReader
{
    Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDetailsDto>> ListAsync(PageRequest page, ProductFilter filter, CancellationToken cancellationToken = default);
}
