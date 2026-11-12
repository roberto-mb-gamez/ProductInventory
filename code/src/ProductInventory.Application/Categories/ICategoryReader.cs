using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Categories;
public interface ICategoryReader
{
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<CategoryDto>> ListAsync(PageRequest page, CancellationToken cancellationToken = default);
}
