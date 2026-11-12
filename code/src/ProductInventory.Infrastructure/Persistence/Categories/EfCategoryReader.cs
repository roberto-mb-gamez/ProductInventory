using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Pagination;
namespace ProductInventory.Infrastructure.Persistence.Categories;
public sealed class EfCategoryReader(InventoryReadDbContext context) : ICategoryReader
{
    public Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return context.Categories.AsNoTracking().Where(row => row.Id == id)
            .Select(row => new CategoryDto(row.Id, row.Name, row.Description)).SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<PagedResult<CategoryDto>> ListAsync(PageRequest page, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        if (page.Offset >= total)
        {

            return new([], page.Page, page.PageSize, total);
        }
        var items = await query.OrderBy(row => row.Name).ThenBy(row => row.Id).Skip((int)page.Offset).Take(page.PageSize)
            .Select(row => new CategoryDto(row.Id, row.Name, row.Description)).ToListAsync(cancellationToken);

        return new(items, page.Page, page.PageSize, total);
    }
}
