namespace ProductInventory.Domain.Products;
public sealed class Product(Guid id, ProductData data)
{
    public Guid Id { get; } = id;
    public ProductData Data { get; } = Validate(data);
    public int StockQuantity => 0;
    private static ProductData Validate(ProductData value)
    {
        DomainRules.RequiredText(value.Sku, 40, "SKU");
        DomainRules.RequiredText(value.Name, 120, "nombre de producto");
        if (value.Price < 0)
            throw new ValidationException("El precio no puede ser negativo.");
        if (value.CategoryId == Guid.Empty || value.SupplierId == Guid.Empty)
            throw new ValidationException("La categoría y el proveedor son obligatorios.");

        return value;
    }
}
