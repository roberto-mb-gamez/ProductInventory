using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Application.Suppliers;
public sealed class CreateSupplierHandler(ISupplierWriter writer)
{
    public Task<SupplierDto> HandleAsync(CreateSupplierCommand request, CancellationToken cancellationToken = default)
    {
        var category = new Supplier(Guid.NewGuid(), request.Data.Name, request.Data.ContactEmail);

        return writer.CreateAsync(category, cancellationToken);
    }
}
