using ProductInventory.Domain.Categories;
namespace ProductInventory.Application.Categories;
public sealed class CreateCategoryHandler(ICategoryWriter writer)
{
    public Task<CategoryDto> HandleAsync(CreateCategoryCommand request, CancellationToken cancellationToken = default)
    {
        var category = new Category(Guid.NewGuid(), request.Data.Name, request.Data.Description);

        return writer.CreateAsync(category, cancellationToken);
    }
}
