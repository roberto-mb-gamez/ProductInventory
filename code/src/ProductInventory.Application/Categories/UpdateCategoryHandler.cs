using ProductInventory.Application.Errors;
using ProductInventory.Domain.Categories;
namespace ProductInventory.Application.Categories;
public sealed class UpdateCategoryHandler(ICategoryWriter writer)
{
    public async Task<CategoryDto> HandleAsync(UpdateCategoryCommand request, CancellationToken cancellationToken = default)
    {
        var category = new Category(request.Id, request.Data.Name, request.Data.Description);

        return await writer.UpdateAsync(category, cancellationToken) ?? throw new NotFoundException("La categoría no existe.");
    }
}
