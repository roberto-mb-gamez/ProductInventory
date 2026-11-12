using ProductInventory.Domain.Products;
namespace ProductInventory.Application.Products;
public sealed class CreateProductHandler(IProductWriter writer)
{
    public Task<ProductDto> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        var data = request.Data;
        var product = new Product(Guid.NewGuid(), new(data.Sku, data.Name, data.Description, data.Price, data.CategoryId, data.SupplierId));

        return writer.CreateAsync(product, cancellationToken);
    }
}
