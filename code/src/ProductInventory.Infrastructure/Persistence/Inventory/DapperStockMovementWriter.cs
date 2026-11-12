using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Inventory;
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

            return await ExecuteAsync(request, cancellationToken);
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
    private async Task<MovementWriteResult> ExecuteAsync(RegisterMovementRequest request, CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        var parameters = new DynamicParameters(new { Id = Guid.NewGuid(), request.ProductId, Type = (int)request.Type, request.Quantity });
        parameters.Add("Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
        var command = new CommandDefinition("dbo.InventoryMovement_Register", parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        var movement = await connection.QuerySingleOrDefaultAsync<StockMovementDto>(command);

        return new(parameters.Get<MovementWriteStatus>("Status"), movement);
    }
}
