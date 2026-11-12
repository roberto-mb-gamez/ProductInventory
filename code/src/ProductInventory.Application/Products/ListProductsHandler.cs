using ProductInventory.Domain;
using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Products;
public sealed class ListProductsHandler(IProductReader reader)
{
    public Task<PagedResult<ProductDetailsDto>> HandleAsync(ListProductsQuery request, CancellationToken cancellationToken = default)
    {
        request.Page.Validate();
        if (request.Filter.MaxStock < 0)
            throw new ValidationException("El filtro de existencias máximas no puede ser negativo.");

        return reader.ListAsync(request.Page, request.Filter, cancellationToken);
    }
}
