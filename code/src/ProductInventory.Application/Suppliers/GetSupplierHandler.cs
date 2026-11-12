using ProductInventory.Application.Errors;
namespace ProductInventory.Application.Suppliers;
public sealed class GetSupplierHandler(ISupplierReader reader)
{
    public async Task<SupplierDto> HandleAsync(GetSupplierQuery request, CancellationToken cancellationToken = default)
    {

        return await reader.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("El proveedor no existe.");
    }
}
