using ProductInventory.Application.Errors;
using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Application.Suppliers;
public sealed class UpdateSupplierHandler(ISupplierWriter writer)
{
    public async Task<SupplierDto> HandleAsync(UpdateSupplierCommand request, CancellationToken cancellationToken = default)
    {
        var category = new Supplier(request.Id, request.Data.Name, request.Data.ContactEmail);

        return await writer.UpdateAsync(category, cancellationToken) ?? throw new NotFoundException("El proveedor no existe.");
    }
}
