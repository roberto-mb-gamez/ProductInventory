using ProductInventory.Application.Errors;
namespace ProductInventory.Application.Products;
public sealed class GetProductHandler(IProductReader reader)
{
    public async Task<ProductDetailsDto> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {

        return await reader.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("El producto no existe.");
    }
}
