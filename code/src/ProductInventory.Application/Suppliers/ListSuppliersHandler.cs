using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Suppliers;
public sealed class ListSuppliersHandler(ISupplierReader reader)
{
    public Task<PagedResult<SupplierDto>> HandleAsync(ListSuppliersQuery request, CancellationToken cancellationToken = default)
    {
        request.Page.Validate();

        return reader.ListAsync(request.Page, cancellationToken);
    }
}
