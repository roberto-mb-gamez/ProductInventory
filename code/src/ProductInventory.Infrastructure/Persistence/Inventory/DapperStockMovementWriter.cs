using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Inventory;
using ProductInventory.Domain.Inventory;
namespace ProductInventory.Infrastructure.Persistence.Inventory;
public sealed class DapperStockMovementWriter(SqlConnectionFactory connections) : IStockMovementWriter
{
    public async Task<MovementWriteResult> RegisterAsync(RegisterMovementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.Quantity);
        if (!Enum.IsDefined(request.Type))
            throw new ArgumentOutOfRangeException(nameof(request), "Movement type must be In or Out.");
        try
        {

            return await ExecuteTransactionAsync(request, cancellationToken);
        }
        catch (SqlException exception) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException("The movement transaction was canceled.", exception, cancellationToken);
        }
        catch (SqlException exception) when (exception.Number == 1205)
        {

            return new MovementWriteResult(MovementWriteStatus.ConcurrencyConflict);
        }
    }
    private async Task<MovementWriteResult> ExecuteTransactionAsync(RegisterMovementRequest request, CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        var result = await RegisterWithinTransactionAsync(connection, transaction, request, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return result;
    }
    private static async Task<MovementWriteResult> RegisterWithinTransactionAsync(SqlConnection connection, SqlTransaction transaction, RegisterMovementRequest request, CancellationToken cancellationToken)
    {
        var balance = await UpdateStockAsync(connection, transaction, request, cancellationToken);
        if (balance is null)
        {

            return await GetFailureAsync(connection, transaction, request, cancellationToken);
        }
        var movement = CreateMovement(request, balance);
        await InsertMovementAsync(connection, transaction, movement, cancellationToken);

        return new MovementWriteResult(MovementWriteStatus.Created, movement);
    }
    private static StockMovementDto CreateMovement(RegisterMovementRequest request, StockBalance balance)
    {
        var movement = new StockMovement(new(request.ProductId, request.Type, request.Quantity), balance.StockBefore);

        return new(movement.Id, movement.ProductId, movement.Type, movement.Quantity, movement.OccurredAt, movement.StockBefore, movement.StockAfter);
    }
    private static Task<StockBalance?> UpdateStockAsync(SqlConnection connection, SqlTransaction transaction, RegisterMovementRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Products
            SET StockQuantity = CONVERT(int, CONVERT(bigint, StockQuantity) + @Delta)
            OUTPUT deleted.StockQuantity AS StockBefore, inserted.StockQuantity AS StockAfter
            WHERE Id = @ProductId AND CONVERT(bigint, StockQuantity) + @Delta BETWEEN 0 AND 2147483647;
            """;
        var parameters = new { request.ProductId, Delta = request.Type == MovementType.In ? request.Quantity : -(long)request.Quantity };

        return connection.QuerySingleOrDefaultAsync<StockBalance>(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
    }
    private static async Task<MovementWriteResult> GetFailureAsync(SqlConnection connection, SqlTransaction transaction, RegisterMovementRequest request, CancellationToken cancellationToken)
    {
        var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SELECT COUNT(*) FROM dbo.Products WHERE Id = @ProductId", new { request.ProductId }, transaction, cancellationToken: cancellationToken));
        if (exists == 0)
        {

            return new MovementWriteResult(MovementWriteStatus.ProductNotFound);
        }
        var status = request.Type == MovementType.Out ? MovementWriteStatus.InsufficientStock : MovementWriteStatus.StockLimitExceeded;

        return new MovementWriteResult(status);
    }
    private static Task InsertMovementAsync(SqlConnection connection, SqlTransaction transaction, StockMovementDto movement, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.InventoryMovements (Id, ProductId, Type, Quantity, OccurredAt, StockBefore, StockAfter)
            VALUES (@Id, @ProductId, @Type, @Quantity, @OccurredAt, @StockBefore, @StockAfter);
            """;
        var parameters = new { movement.Id, movement.ProductId, Type = movement.Type == MovementType.In ? "IN" : "OUT",
            movement.Quantity, movement.OccurredAt, movement.StockBefore, movement.StockAfter };

        return connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
    }
}
