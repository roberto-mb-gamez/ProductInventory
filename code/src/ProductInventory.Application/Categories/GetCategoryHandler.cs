using ProductInventory.Application.Errors;
namespace ProductInventory.Application.Categories;
public sealed class GetCategoryHandler(ICategoryReader reader)
{
    public async Task<CategoryDto> HandleAsync(GetCategoryQuery request, CancellationToken cancellationToken = default)
    {

        return await reader.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("La categoría no existe.");
    }
}
