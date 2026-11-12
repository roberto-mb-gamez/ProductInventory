using System.Net;
using System.Net.Http.Json;
using Dapper;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Pagination;
using ProductInventory.IntegrationTests.Persistence;
namespace ProductInventory.IntegrationTests.Api;
[Collection("SqlServer")]
public sealed class CategoryApiTests(SqlServerFixture database)
{
    [Fact]
    public async Task UC008CreatesCategoryAndRejectsDuplicateIgnoringCase()
    {
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var name = $"Category-{Guid.NewGuid():N}";
        using var created = await client.PostAsJsonAsync("/api/categories", new CategoryInput(name, "Description"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var category = await created.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.Name);
        Assert.EndsWith(category.Id.ToString(), created.Headers.Location!.ToString());
        using var duplicate = await client.PostAsJsonAsync("/api/categories", new CategoryInput(name.ToLowerInvariant(), null));
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }
    [Fact]
    public async Task UC009ReturnsExistingCategoryAndNotFoundForMissingCategory()
    {
        var id = await SeedCategoryAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var category = await client.GetFromJsonAsync<CategoryDto>($"/api/categories/{id}");
        Assert.Equal(id, category!.Id);
        using var missing = await client.GetAsync($"/api/categories/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
    [Fact]
    public async Task UC010ReturnsDeterministicPageAndValidatesBounds()
    {
        await SeedCategoryAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var page = await client.GetFromJsonAsync<PagedResult<CategoryDto>>("/api/categories?page=1&pageSize=2");
        await using var connection = await database.Connections.OpenAsync();
        var expected = await connection.QueryAsync<Guid>("SELECT TOP (2) Id FROM dbo.Categories ORDER BY Name, Id");
        Assert.Equal(expected, page!.Items.Select(item => item.Id));
        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Categories"), page.TotalCount);
        using var invalid = await client.GetAsync("/api/categories?page=0&pageSize=101");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        var empty = await client.GetFromJsonAsync<PagedResult<CategoryDto>>("/api/categories?page=2147483647&pageSize=100");
        Assert.Empty(empty!.Items);
    }
    [Fact]
    public async Task UC011UpdatesCategoryAndRejectsMissingResource()
    {
        var id = await SeedCategoryAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.PutAsJsonAsync($"/api/categories/{id}", new CategoryInput($"Updated-{id:N}", "Updated"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.Equal(id, category!.Id);
        Assert.Equal("Updated", category.Description);
        using var missing = await client.PutAsJsonAsync($"/api/categories/{Guid.NewGuid()}", new CategoryInput("Missing", null));
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
    [Fact]
    public async Task UC012DeletesUnreferencedCategoryAndRejectsMissingResource()
    {
        var id = await SeedCategoryAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.DeleteAsync($"/api/categories/{id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var missing = await client.DeleteAsync($"/api/categories/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
    [Fact]
    public async Task UC012ProtectsCategoryWithProducts()
    {
        var productId = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var categoryId = await connection.ExecuteScalarAsync<Guid>("SELECT CategoryId FROM dbo.Products WHERE Id=@Id", new { Id = productId });
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.DeleteAsync($"/api/categories/{categoryId}");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Products WHERE Id=@Id", new { Id = productId }));
    }
    [Theory]
    [InlineData("")]
    [InlineData(" \t ")]
    public async Task UC008AndUC011RejectInvalidNames(string name)
    {
        var id = await SeedCategoryAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var created = await client.PostAsJsonAsync("/api/categories", new CategoryInput(name, null));
        using var updated = await client.PutAsJsonAsync($"/api/categories/{id}", new CategoryInput(name, null));
        Assert.Equal(HttpStatusCode.BadRequest, created.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, updated.StatusCode);
    }
    private async Task<Guid> SeedCategoryAsync()
    {
        var id = Guid.NewGuid();
        await using var connection = await database.Connections.OpenAsync();
        await connection.ExecuteAsync("INSERT dbo.Categories(Id,Name) VALUES(@Id,@Name)", new { Id = id, Name = $"Category-{id:N}" });

        return id;
    }
}
