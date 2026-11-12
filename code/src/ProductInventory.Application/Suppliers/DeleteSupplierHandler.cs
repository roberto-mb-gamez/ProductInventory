using ProductInventory.Application.Errors;
namespace ProductInventory.Application.Suppliers;
public sealed class DeleteSupplierHandler(ISupplierWriter writer)
{
    public async Task HandleAsync(DeleteSupplierCommand request, CancellationToken cancellationToken = default)
    {
        if (!await writer.DeleteAsync(request.Id, cancellationToken))
            throw new NotFoundException("El proveedor no existe.");
    }
}
