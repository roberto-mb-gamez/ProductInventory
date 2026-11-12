using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Pagination;
namespace ProductInventory.Infrastructure.Persistence.Suppliers;
public sealed class EfSupplierReader(InventoryReadDbContext context) : ISupplierReader
{
    public Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return context.Suppliers.AsNoTracking().Where(row => row.Id == id)
            .Select(row => new SupplierDto(row.Id, row.Name, row.ContactEmail)).SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<PagedResult<SupplierDto>> ListAsync(PageRequest page, CancellationToken cancellationToken = default)
    {
        var query = context.Suppliers.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        if (page.Offset >= total)
        {

            return new([], page.Page, page.PageSize, total);
        }
        var items = await query.OrderBy(row => row.Name).ThenBy(row => row.Id).Skip((int)page.Offset).Take(page.PageSize)
            .Select(row => new SupplierDto(row.Id, row.Name, row.ContactEmail)).ToListAsync(cancellationToken);

        return new(items, page.Page, page.PageSize, total);
    }
}
