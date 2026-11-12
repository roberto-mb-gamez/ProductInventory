using Microsoft.AspNetCore.Mvc;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Pagination;
namespace ProductInventory.Api.Features.Suppliers;
public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/suppliers").WithTags("Suppliers").RequireAuthorization();
        group.MapPost("", CreateAsync).Produces<SupplierDto>(201).ProducesProblem(400).ProducesProblem(409);
        group.MapGet("/{id:guid}", GetAsync).Produces<SupplierDto>().ProducesProblem(404);
        group.MapGet("", ListAsync).Produces<PagedResult<SupplierDto>>().ProducesProblem(400);
        group.MapPut("/{id:guid}", UpdateAsync).Produces<SupplierDto>().ProducesProblem(400).ProducesProblem(404).ProducesProblem(409);
        group.MapDelete("/{id:guid}", DeleteAsync).Produces(204).ProducesProblem(404).ProducesProblem(409);
    }
    private static async Task<IResult> CreateAsync(SupplierInput data, CreateSupplierHandler handler, CancellationToken cancellationToken)
    {
        var supplier = await handler.HandleAsync(new(data), cancellationToken);

        return Results.Created($"/api/suppliers/{supplier.Id}", supplier);
    }
    private static async Task<IResult> GetAsync(Guid id, GetSupplierHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(id), cancellationToken));
    }
    private static async Task<IResult> ListAsync([AsParameters] PageRequest page, ListSuppliersHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(page), cancellationToken));
    }
    private static async Task<IResult> UpdateAsync(Guid id, SupplierInput data, UpdateSupplierHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(id, data), cancellationToken));
    }
    private static async Task<IResult> DeleteAsync(Guid id, DeleteSupplierHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new(id), cancellationToken);

        return Results.NoContent();
    }
}
