using ProductInventory.Application.Errors;
namespace ProductInventory.Application.Categories;
public sealed class DeleteCategoryHandler(ICategoryWriter writer)
{
    public async Task HandleAsync(DeleteCategoryCommand request, CancellationToken cancellationToken = default)
    {
        if (!await writer.DeleteAsync(request.Id, cancellationToken))
            throw new NotFoundException("La categoría no existe.");
    }
}
