namespace ProductInventory.Infrastructure.Persistence.ReadModels;
public sealed class SupplierRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
}
