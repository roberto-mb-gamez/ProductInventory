using Microsoft.AspNetCore.Mvc;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Pagination;
namespace ProductInventory.Api.Features.Categories;
public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories").RequireAuthorization();
        group.MapPost("", CreateAsync).Produces<CategoryDto>(201).ProducesProblem(400).ProducesProblem(409);
        group.MapGet("/{id:guid}", GetAsync).Produces<CategoryDto>().ProducesProblem(404);
        group.MapGet("", ListAsync).Produces<PagedResult<CategoryDto>>().ProducesProblem(400);
        group.MapPut("/{id:guid}", UpdateAsync).Produces<CategoryDto>().ProducesProblem(400).ProducesProblem(404).ProducesProblem(409);
        group.MapDelete("/{id:guid}", DeleteAsync).Produces(204).ProducesProblem(404).ProducesProblem(409);
    }
    private static async Task<IResult> CreateAsync(CategoryInput data, CreateCategoryHandler handler, CancellationToken cancellationToken)
    {
        var category = await handler.HandleAsync(new(data), cancellationToken);

        return Results.Created($"/api/categories/{category.Id}", category);
    }
    private static async Task<IResult> GetAsync(Guid id, GetCategoryHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(id), cancellationToken));
    }
    private static async Task<IResult> ListAsync([AsParameters] PageRequest page, ListCategoriesHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(page), cancellationToken));
    }
    private static async Task<IResult> UpdateAsync(Guid id, CategoryInput data, UpdateCategoryHandler handler, CancellationToken cancellationToken)
    {

        return Results.Ok(await handler.HandleAsync(new(id, data), cancellationToken));
    }
    private static async Task<IResult> DeleteAsync(Guid id, DeleteCategoryHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new(id), cancellationToken);

        return Results.NoContent();
    }
}
