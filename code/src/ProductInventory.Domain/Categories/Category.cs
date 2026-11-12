namespace ProductInventory.Domain.Categories;
public sealed class Category(Guid id, string name, string? description)
{
    public Guid Id { get; } = id;
    public string Name { get; } = DomainRules.RequiredText(name, 100, "nombre de categoría");
    public string? Description { get; } = description;
}
