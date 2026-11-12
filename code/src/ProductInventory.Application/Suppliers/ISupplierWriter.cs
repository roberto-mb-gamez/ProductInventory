using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Application.Suppliers;
public interface ISupplierWriter
{
    Task<SupplierDto> CreateAsync(Supplier category, CancellationToken cancellationToken = default);
    Task<SupplierDto?> UpdateAsync(Supplier category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
