namespace ProductInventory.Infrastructure.Persistence.ReadModels;
public sealed class CategoryRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
