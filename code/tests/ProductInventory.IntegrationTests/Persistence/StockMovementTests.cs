using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Inventory;
using ProductInventory.Domain.Inventory;
using ProductInventory.Infrastructure.Persistence.Inventory;
namespace ProductInventory.IntegrationTests.Persistence;
[Collection("SqlServer")]
public sealed class StockMovementTests(SqlServerFixture database)
{
    private DapperStockMovementWriter CreateWriter() => new(database.Connections);
    [Fact]
    public async Task InAndOutPersistUtcAndBeforeAfterBalances()
    {
        var id = await database.CreateProductAsync();
        var writer = CreateWriter();
        var incoming = await writer.RegisterAsync(new(id, MovementType.In, 7));
        var outgoing = await writer.RegisterAsync(new(id, MovementType.Out, 3));
        Assert.Equal(MovementWriteStatus.Created, incoming.Status);
        Assert.Equal(MovementWriteStatus.Created, outgoing.Status);
        Assert.Equal(0, incoming.Movement!.StockBefore);
        Assert.Equal(7, incoming.Movement.StockAfter);
        Assert.Equal(7, outgoing.Movement!.StockBefore);
        Assert.Equal(4, outgoing.Movement.StockAfter);
        await using var context = database.CreateReadContext();
        Assert.Equal(4, await context.Products.Where(row => row.Id == id).Select(row => row.StockQuantity).SingleAsync());
        var movements = await context.InventoryMovements.Where(row => row.ProductId == id).ToListAsync();
        Assert.Equal(2, movements.Count);
        Assert.All(movements, movement => Assert.Equal(TimeSpan.Zero, movement.OccurredAt.Offset));
        Assert.Contains(movements, movement => movement.Id == outgoing.Movement.Id && movement.Type == "OUT" && movement.Quantity == 3);
    }
    [Fact]
    public async Task MissingProductAndInsufficientStockHaveNoPartialEffects()
    {
        var id = await database.CreateProductAsync();
        var writer = CreateWriter();
        var missing = await writer.RegisterAsync(new(Guid.NewGuid(), MovementType.In, 1));
        var insufficient = await writer.RegisterAsync(new(id, MovementType.Out, 1));
        Assert.Equal(MovementWriteStatus.ProductNotFound, missing.Status);
        Assert.Equal(MovementWriteStatus.InsufficientStock, insufficient.Status);
        await AssertStateAsync(id, 0, 0);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task NonPositiveQuantitiesDoNotChangeStock(int quantity)
    {
        var id = await database.CreateProductAsync();
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateWriter().RegisterAsync(new(id, MovementType.In, quantity)));
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task UnknownMovementTypeDoesNotChangeStock()
    {
        var id = await database.CreateProductAsync();
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateWriter().RegisterAsync(new(id, (MovementType)99, 1)));
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task StockOverflowIsRejectedWithoutPartialEffects()
    {
        var id = await database.CreateProductAsync();
        var writer = CreateWriter();
        Assert.Equal(MovementWriteStatus.Created, (await writer.RegisterAsync(new(id, MovementType.In, int.MaxValue))).Status);
        Assert.Equal(MovementWriteStatus.StockLimitExceeded, (await writer.RegisterAsync(new(id, MovementType.In, 1))).Status);
        await AssertStateAsync(id, int.MaxValue, 1);
    }
    [Fact]
    public async Task FailedMovementInsertRollsBackTheStockUpdate()
    {
        var id = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var trigger = $"TR_TestReject_{id:N}";
        await connection.ExecuteAsync($"""
            CREATE TRIGGER dbo.[{trigger}] ON dbo.InventoryMovements AFTER INSERT AS
            IF EXISTS (SELECT 1 FROM inserted WHERE ProductId = '{id}') THROW 51002, 'Injected movement insert failure.', 1;
            """);
        await Assert.ThrowsAsync<SqlException>(() => CreateWriter().RegisterAsync(new(id, MovementType.In, 5)));
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task CancellationDuringInsertRollsBackStockAndMovement()
    {
        var id = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var resource = $"MovementCancellation_{id:N}";
        await connection.ExecuteAsync($"""
            CREATE TRIGGER dbo.[TR_TestCancel_{id:N}] ON dbo.InventoryMovements AFTER INSERT AS
            IF EXISTS (SELECT 1 FROM inserted WHERE ProductId = '{id}')
            BEGIN
                EXEC sys.sp_getapplock @Resource = '{resource}', @LockMode = 'Exclusive', @LockOwner = 'Transaction';
                WAITFOR DELAY '00:00:10';
            END;
            """);
        using var cancellation = new CancellationTokenSource();
        var operation = CreateWriter().RegisterAsync(new(id, MovementType.In, 3), cancellation.Token);
        await WaitForTransactionLockAsync(resource);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task ConcurrentInputsDoNotLoseUpdates()
    {
        var id = await database.CreateProductAsync();
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, 20).Select(async _ =>
        {
            await start.Task;

            return await CreateWriter().RegisterAsync(new(id, MovementType.In, 1));
        }).ToArray();
        start.SetResult();
        var results = await Task.WhenAll(tasks);
        Assert.All(results, result => Assert.Equal(MovementWriteStatus.Created, result.Status));
        Assert.Equal(Enumerable.Range(1, 20), results.Select(result => result.Movement!.StockAfter).Order());
        await AssertStateAsync(id, 20, 20);
    }
    [Fact]
    public async Task ConcurrentOutputsNeverOversell()
    {
        var id = await database.CreateProductAsync();
        await CreateWriter().RegisterAsync(new(id, MovementType.In, 10));
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, 20).Select(async _ =>
        {
            await start.Task;

            return await CreateWriter().RegisterAsync(new(id, MovementType.Out, 1));
        }).ToArray();
        start.SetResult();
        var results = await Task.WhenAll(tasks);
        Assert.Equal(10, results.Count(result => result.Status == MovementWriteStatus.Created));
        Assert.Equal(10, results.Count(result => result.Status == MovementWriteStatus.InsufficientStock));
        await AssertStateAsync(id, 0, 11);
    }
    [Fact]
    public async Task HistoryCannotBeEditedDeletedOrCascadedWithProductDeletion()
    {
        var id = await database.CreateProductAsync();
        await CreateWriter().RegisterAsync(new(id, MovementType.In, 1));
        await using var connection = await database.Connections.OpenAsync();
        var updateError = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(
            "UPDATE dbo.InventoryMovements SET Quantity = 2, StockAfter = 2 WHERE ProductId = @Id", new { Id = id }));
        var deleteError = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(
            "DELETE FROM dbo.InventoryMovements WHERE ProductId = @Id", new { Id = id }));
        var productError = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(
            "DELETE FROM dbo.Products WHERE Id = @Id", new { Id = id }));
        Assert.Equal(51001, updateError.Number);
        Assert.Equal(51001, deleteError.Number);
        Assert.Equal(547, productError.Number);
        await AssertStateAsync(id, 1, 1);
    }
    private async Task AssertStateAsync(Guid id, int stock, int movementCount)
    {
        await using var context = database.CreateReadContext();
        Assert.Equal(stock, await context.Products.Where(row => row.Id == id).Select(row => row.StockQuantity).SingleAsync());
        Assert.Equal(movementCount, await context.InventoryMovements.CountAsync(row => row.ProductId == id));
    }
    private async Task WaitForTransactionLockAsync(string resource)
    {
        await using var connection = await database.Connections.OpenAsync();
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var available = await connection.ExecuteScalarAsync<int>(
                "SELECT APPLOCK_TEST('public', @Resource, 'Shared', 'Session')", new { Resource = resource });
            if (available == 0)
            {

                return;
            }
            await Task.Delay(20);
        }
        throw new TimeoutException("Movement insert did not reach the cancellation checkpoint.");
    }
}
