using Moq;
using ProductInventory.Application.Products;
using ProductInventory.Application.Errors;
using ProductInventory.Domain;
using ProductInventory.Domain.Products;
namespace ProductInventory.Application.Tests.Products;
public sealed class ProductHandlerTests
{
    private static CreateProductRequest ValidInput => new("SKU-1", "Product", null, 1, Guid.NewGuid(), Guid.NewGuid());
    [Fact]
    public async Task CreateValidatesBeforeWriting()
    {
        var writer = new Mock<IProductWriter>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new CreateProductHandler(writer.Object).HandleAsync(new(ValidInput with { Price = -1 })));
        writer.VerifyNoOtherCalls();
    }
    [Fact]
    public async Task CreateWritesValidatedProductWithZeroStock()
    {
        var writer = new Mock<IProductWriter>();
        writer.Setup(port => port.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product product, CancellationToken _) => new ProductDto(product.Id, product.Data.Sku, product.Data.Name,
                product.Data.Description, product.Data.Price, product.StockQuantity, product.Data.CategoryId, product.Data.SupplierId));
        var product = await new CreateProductHandler(writer.Object).HandleAsync(new(ValidInput));
        Assert.Equal(0, product.StockQuantity);
        writer.Verify(port => port.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task GetUpdateAndDeleteRejectMissingProducts()
    {
        var writer = new Mock<IProductWriter>();
        var reader = new Mock<IProductReader>();
        await Assert.ThrowsAsync<NotFoundException>(() => new GetProductHandler(reader.Object).HandleAsync(new(Guid.NewGuid())));
        await Assert.ThrowsAsync<NotFoundException>(() => new UpdateProductHandler(writer.Object).HandleAsync(new(Guid.NewGuid(), ValidInput)));
        await Assert.ThrowsAsync<NotFoundException>(() => new DeleteProductHandler(writer.Object).HandleAsync(new(Guid.NewGuid())));
    }
    [Fact]
    public async Task InvalidFiltersNeverCallReader()
    {
        var reader = new Mock<IProductReader>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new ListProductsHandler(reader.Object).HandleAsync(new(new(), new(MaxStock: -1))));
        reader.VerifyNoOtherCalls();
    }
}
