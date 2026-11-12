using ProductInventory.Application.Errors;
namespace ProductInventory.Application.Products;
public sealed class DeleteProductHandler(IProductWriter writer)
{
    public async Task HandleAsync(DeleteProductCommand request, CancellationToken cancellationToken = default)
    {
        if (!await writer.DeleteAsync(request.Id, cancellationToken))
            throw new NotFoundException("El producto no existe.");
    }
}
