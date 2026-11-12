using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Products;
namespace ProductInventory.Infrastructure.Persistence.Products;
public sealed class EfProductReader(InventoryReadDbContext context) : IProductReader
{
    public Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return context.Products.AsNoTracking().Where(product => product.Id == id)
            .Select(product => new ProductDetailsDto(product.Id, product.Sku, product.Name, product.Description,
                product.Price, product.StockQuantity, product.CategoryId, product.Category.Name, product.SupplierId, product.Supplier.Name))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
