using ProductInventory.Domain;
using ProductInventory.Domain.Products;
namespace ProductInventory.Domain.Tests.Products;
public sealed class ProductTests
{
    private static ProductData ValidData => new("SKU-1", "Product", null, 0, Guid.NewGuid(), Guid.NewGuid());
    [Fact]
    public void NewProductAlwaysStartsWithZeroStock()
    {
        Assert.Equal(0, new Product(Guid.NewGuid(), ValidData).StockQuantity);
    }
    [Fact]
    public void ProductRejectsInvalidTextLengths()
    {
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { Sku = " " }));
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { Sku = new string('s', 41) }));
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { Name = new string('n', 121) }));
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { Name = "\t" }));
    }
    [Fact]
    public void ProductRejectsNegativePriceAndMissingReferences()
    {
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { Price = -1 }));
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { CategoryId = Guid.Empty }));
        Assert.Throws<ValidationException>(() => new Product(Guid.NewGuid(), ValidData with { SupplierId = Guid.Empty }));
    }
}
