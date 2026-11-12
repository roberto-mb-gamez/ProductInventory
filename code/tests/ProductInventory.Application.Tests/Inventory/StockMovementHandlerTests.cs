using Moq;
using ProductInventory.Application.Inventory;
using ProductInventory.Application.Errors;
using ProductInventory.Domain;
using ProductInventory.Domain.Inventory;
namespace ProductInventory.Application.Tests.Inventory;
public sealed class StockMovementHandlerTests
{
    [Theory]
    [InlineData("BAD", 1)]
    [InlineData("in", 1)]
    [InlineData("IN", 0)]
    [InlineData("OUT", -1)]
    public async Task InvalidMovementNeverCallsWriter(string type, int quantity)
    {
        var writer = new Mock<IStockMovementWriter>(MockBehavior.Strict);
        await Assert.ThrowsAsync<ValidationException>(() => new RegisterStockMovementHandler(writer.Object).HandleAsync(new(Guid.NewGuid(), new(type, quantity))));
        writer.VerifyNoOtherCalls();
    }
    [Fact]
    public async Task MissingProductMapsToNotFound()
    {
        var writer = new Mock<IStockMovementWriter>();
        writer.Setup(port => port.RegisterAsync(It.IsAny<RegisterMovementRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovementWriteResult(MovementWriteStatus.ProductNotFound));
        await Assert.ThrowsAsync<NotFoundException>(() => new RegisterStockMovementHandler(writer.Object).HandleAsync(new(Guid.NewGuid(), new("IN", 1))));
    }
    [Theory]
    [InlineData(MovementWriteStatus.InsufficientStock)]
    [InlineData(MovementWriteStatus.StockLimitExceeded)]
    [InlineData(MovementWriteStatus.ConcurrencyConflict)]
    public async Task StockFailuresMapToConflict(MovementWriteStatus status)
    {
        var writer = new Mock<IStockMovementWriter>();
        writer.Setup(port => port.RegisterAsync(It.IsAny<RegisterMovementRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovementWriteResult(status));
        await Assert.ThrowsAsync<ConflictException>(() => new RegisterStockMovementHandler(writer.Object).HandleAsync(new(Guid.NewGuid(), new("OUT", 1))));
    }
    [Fact]
    public async Task ValidMovementReturnsImmutableHistory()
    {
        var id = Guid.NewGuid();
        var movement = new StockMovementDto(Guid.NewGuid(), id, MovementType.In, 2, DateTimeOffset.UtcNow, 0, 2);
        var writer = new Mock<IStockMovementWriter>();
        writer.Setup(port => port.RegisterAsync(new RegisterMovementRequest(id, MovementType.In, 2), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovementWriteResult(MovementWriteStatus.Created, movement));
        Assert.Equal(movement, await new RegisterStockMovementHandler(writer.Object).HandleAsync(new(id, new("IN", 2))));
    }
}
