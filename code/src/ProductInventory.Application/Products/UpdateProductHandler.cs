using ProductInventory.Application.Errors;
using ProductInventory.Domain.Products;
namespace ProductInventory.Application.Products;
public sealed class UpdateProductHandler(IProductWriter writer)
{
    public async Task<ProductDto> HandleAsync(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        var data = request.Data;
        var product = new Product(request.Id, new(data.Sku, data.Name, data.Description, data.Price, data.CategoryId, data.SupplierId));

        return await writer.UpdateAsync(product, cancellationToken) ?? throw new NotFoundException("El producto no existe.");
    }
}
