using ProductInventory.Application.Errors;
using ProductInventory.Domain;
using ProductInventory.Domain.Inventory;
namespace ProductInventory.Application.Inventory;
public sealed class RegisterStockMovementHandler(IStockMovementWriter writer)
{
    public async Task<StockMovementDto> HandleAsync(RegisterStockMovementCommand request, CancellationToken cancellationToken = default)
    {
        var type = request.Data.Type switch
        {
            "IN" => MovementType.In,
            "OUT" => MovementType.Out,
            _ => throw new ValidationException("El tipo de movimiento debe ser IN u OUT.")
        };
        StockMovement.Validate(new(request.ProductId, type, request.Data.Quantity));
        var result = await writer.RegisterAsync(new(request.ProductId, type, request.Data.Quantity), cancellationToken);

        return result.Status switch
        {
            MovementWriteStatus.Created => result.Movement ?? throw new InvalidOperationException("Created movement data is missing."),
            MovementWriteStatus.ProductNotFound => throw new NotFoundException("El producto no existe."),
            MovementWriteStatus.InsufficientStock => throw new ConflictException("Las existencias son insuficientes."),
            MovementWriteStatus.StockLimitExceeded => throw new ConflictException("El movimiento supera el límite de existencias."),
            MovementWriteStatus.ConcurrencyConflict => throw new ConflictException("Se produjo un conflicto de concurrencia."),
            _ => throw new InvalidOperationException("Unknown movement result.")
        };
    }
}
