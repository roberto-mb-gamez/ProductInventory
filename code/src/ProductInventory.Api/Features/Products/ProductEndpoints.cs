using Microsoft.AspNetCore.Mvc;
using ProductInventory.Application.Products;
using ProductInventory.Application.Pagination;
namespace ProductInventory.Api.Features.Products;
public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products").RequireAuthorization();
        group.MapPost("", CreateAsync).Produces<ProductDto>(201).ProducesProblem(400).ProducesProblem(404).ProducesProblem(409);
        group.MapGet("/{id:guid}", GetAsync).Produces<ProductDetailsDto>().ProducesProblem(404);
        group.MapGet("", ListAsync).Produces<PagedResult<ProductDetailsDto>>().ProducesProblem(400);
        group.MapPut("/{id:guid}", UpdateAsync).Produces<ProductDto>().ProducesProblem(400).ProducesProblem(404).ProducesProblem(409);
        group.MapDelete("/{id:guid}", DeleteAsync).Produces(204).ProducesProblem(404).ProducesProblem(409);
    }
    private static async Task<IResult> CreateAsync(CreateProductRequest data, CreateProductHandler handler, CancellationToken cancellationToken)
    {
        var product = await handler.HandleAsync(new(data), cancellationToken);

        return Results.Created($"/api/products/{product.Id}", product);
    }
    private static async Task<IResult> GetAsync(Guid id, GetProductHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(id), cancellationToken));
    }
    private static async Task<IResult> ListAsync([AsParameters] PageRequest page, [AsParameters] ProductFilter filter, ListProductsHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(page, filter), cancellationToken));
    }
    private static async Task<IResult> UpdateAsync(Guid id, CreateProductRequest data, UpdateProductHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(id, data), cancellationToken));
    }
    private static async Task<IResult> DeleteAsync(Guid id, DeleteProductHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new(id), cancellationToken);

        return Results.NoContent();
    }
}
