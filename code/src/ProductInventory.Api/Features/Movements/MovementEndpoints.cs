using ProductInventory.Application.Inventory;
namespace ProductInventory.Api.Features.Movements;
public static class MovementEndpoints
{
    public static void MapMovementEndpoints(this WebApplication app)
    {
        app.MapPost("/api/products/{id:guid}/movements", RegisterAsync).WithTags("Inventory movements").RequireAuthorization()
            .Produces<StockMovementResponse>(201).ProducesProblem(400).ProducesProblem(404).ProducesProblem(409);
    }
    private static async Task<IResult> RegisterAsync(Guid id, StockMovementInput data, RegisterStockMovementHandler handler, CancellationToken cancellationToken)
    {
        var movement = await handler.HandleAsync(new(id, data), cancellationToken);

        return Results.Json(StockMovementResponse.From(movement), statusCode: StatusCodes.Status201Created);
    }
}
