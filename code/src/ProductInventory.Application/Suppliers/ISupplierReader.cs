using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Suppliers;
public interface ISupplierReader
{
    Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<SupplierDto>> ListAsync(PageRequest page, CancellationToken cancellationToken = default);
}
