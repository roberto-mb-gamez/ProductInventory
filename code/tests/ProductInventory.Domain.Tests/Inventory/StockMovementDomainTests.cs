using ProductInventory.Domain;
using ProductInventory.Domain.Inventory;
namespace ProductInventory.Domain.Tests.Inventory;
public sealed class StockMovementDomainTests
{
    [Theory]
    [InlineData(MovementType.In, 5, 3, 8)]
    [InlineData(MovementType.Out, 2, 3, 1)]
    public void MovementComputesBalancesAndUtcTimestamp(MovementType type, int quantity, int stockBefore, int stockAfter)
    {
        var movement = new StockMovement(new(Guid.NewGuid(), type, quantity), stockBefore);
        Assert.Equal(stockBefore, movement.StockBefore);
        Assert.Equal(stockAfter, movement.StockAfter);
        Assert.Equal(TimeSpan.Zero, movement.OccurredAt.Offset);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MovementRejectsNonPositiveQuantity(int quantity)
    {
        Assert.Throws<ValidationException>(() => new StockMovement(new(Guid.NewGuid(), MovementType.In, quantity), 0));
    }
    [Fact]
    public void MovementRejectsUnknownTypeInsufficientStockAndOverflow()
    {
        Assert.Throws<ValidationException>(() => new StockMovement(new(Guid.NewGuid(), (MovementType)99, 1), 0));
        Assert.Throws<StockConflictException>(() => new StockMovement(new(Guid.NewGuid(), MovementType.Out, 1), 0));
        Assert.Throws<StockConflictException>(() => new StockMovement(new(Guid.NewGuid(), MovementType.In, 1), int.MaxValue));
    }
}
