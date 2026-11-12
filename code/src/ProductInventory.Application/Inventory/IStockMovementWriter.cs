namespace ProductInventory.Application.Inventory;
public interface IStockMovementWriter
{
    Task<MovementWriteResult> RegisterAsync(RegisterMovementRequest request, CancellationToken cancellationToken = default);
}
