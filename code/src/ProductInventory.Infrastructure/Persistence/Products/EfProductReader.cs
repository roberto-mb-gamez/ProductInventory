using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Products;
using ProductInventory.Application.Pagination;
using ProductInventory.Infrastructure.Persistence.ReadModels;
namespace ProductInventory.Infrastructure.Persistence.Products;
public sealed class EfProductReader(InventoryReadDbContext context) : IProductReader
{
    public Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return Project(context.Products.AsNoTracking().Where(product => product.Id == id)).SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<PagedResult<ProductDetailsDto>> ListAsync(PageRequest page, ProductFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Filter(context.Products.AsNoTracking(), filter);
        var total = await query.CountAsync(cancellationToken);
        if (page.Offset >= total)
        {

            return new([], page.Page, page.PageSize, total);
        }
        var items = await Project(query.OrderBy(product => product.Sku).ThenBy(product => product.Id)
            .Skip((int)page.Offset).Take(page.PageSize)).ToListAsync(cancellationToken);

        return new(items, page.Page, page.PageSize, total);
    }
    private static IQueryable<ProductRow> Filter(IQueryable<ProductRow> query, ProductFilter filter)
    {
        if (filter.CategoryId is { } categoryId)
            query = query.Where(product => product.CategoryId == categoryId);
        if (filter.SupplierId is { } supplierId)
            query = query.Where(product => product.SupplierId == supplierId);
        if (filter.MaxStock is { } maxStock)
            query = query.Where(product => product.StockQuantity <= maxStock);

        return query;
    }
    private static IQueryable<ProductDetailsDto> Project(IQueryable<ProductRow> query)
    {

        return query.Select(product => new ProductDetailsDto(product.Id, product.Sku, product.Name, product.Description,
            product.Price, product.StockQuantity, product.CategoryId, product.Category.Name, product.SupplierId, product.Supplier.Name));
    }
}
