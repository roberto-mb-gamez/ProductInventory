using Moq;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Errors;
using ProductInventory.Application.Pagination;
using ProductInventory.Domain;
using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Application.Tests.Suppliers;
public sealed class SupplierHandlerTests
{
    [Fact]
    public async Task CreateDelegatesValidatedDataToWriter()
    {
        var writer = new Mock<ISupplierWriter>();
        writer.Setup(port => port.CreateAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier supplier, CancellationToken _) => new SupplierDto(supplier.Id, supplier.Name, supplier.ContactEmail));
        var result = await new CreateSupplierHandler(writer.Object).HandleAsync(new(new("Tools", null)));
        Assert.Equal("Tools", result.Name);
        writer.Verify(port => port.CreateAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task InvalidCreateNeverCallsPersistence()
    {
        var writer = new Mock<ISupplierWriter>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new CreateSupplierHandler(writer.Object).HandleAsync(new(new(" ", null))));
        writer.VerifyNoOtherCalls();
    }
    [Fact]
    public async Task MissingGetAndUpdateReturnNotFound()
    {
        var reader = new Mock<ISupplierReader>();
        var writer = new Mock<ISupplierWriter>();
        await Assert.ThrowsAsync<NotFoundException>(() => new GetSupplierHandler(reader.Object).HandleAsync(new(Guid.NewGuid())));
        await Assert.ThrowsAsync<NotFoundException>(() => new UpdateSupplierHandler(writer.Object).HandleAsync(new(Guid.NewGuid(), new("Tools", null))));
    }
    [Fact]
    public async Task MissingDeleteReturnsNotFound()
    {
        var writer = new Mock<ISupplierWriter>();
        await Assert.ThrowsAsync<NotFoundException>(() => new DeleteSupplierHandler(writer.Object).HandleAsync(new(Guid.NewGuid())));
    }
    [Fact]
    public async Task InvalidPageNeverCallsReader()
    {
        var reader = new Mock<ISupplierReader>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new ListSuppliersHandler(reader.Object).HandleAsync(new(new PageRequest(0, 20))));
        reader.VerifyNoOtherCalls();
    }
}
