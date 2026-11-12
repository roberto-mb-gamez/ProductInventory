namespace ProductInventory.Application.Categories;
public sealed record UpdateCategoryCommand(Guid Id, CategoryInput Data);
