using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Categories;
public sealed class ListCategoriesHandler(ICategoryReader reader)
{
    public Task<PagedResult<CategoryDto>> HandleAsync(ListCategoriesQuery request, CancellationToken cancellationToken = default)
    {
        request.Page.Validate();

        return reader.ListAsync(request.Page, cancellationToken);
    }
}
