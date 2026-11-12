using ProductInventory.Domain.Categories;
namespace ProductInventory.Application.Categories;
public interface ICategoryWriter
{
    Task<CategoryDto> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<CategoryDto?> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
