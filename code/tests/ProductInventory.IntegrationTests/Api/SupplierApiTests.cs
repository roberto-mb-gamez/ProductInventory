using System.Net;
using System.Net.Http.Json;
using Dapper;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Pagination;
using ProductInventory.IntegrationTests.Persistence;
namespace ProductInventory.IntegrationTests.Api;
[Collection("SqlServer")]
public sealed class SupplierApiTests(SqlServerFixture database)
{
    [Fact]
    public async Task UC013CreatesSuppliersWithDuplicateNamesAllowed()
    {
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var data = new SupplierInput($"Supplier-{Guid.NewGuid():N}", "contact@example.test");
        using var first = await client.PostAsJsonAsync("/api/suppliers", data);
        using var second = await client.PostAsJsonAsync("/api/suppliers", data);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var supplier = await first.Content.ReadFromJsonAsync<SupplierDto>();
        Assert.Equal(data.ContactEmail, supplier!.ContactEmail);
    }
    [Fact]
    public async Task UC014ReturnsSupplierAndRejectsMissingResource()
    {
        var id = await SeedSupplierAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var supplier = await client.GetFromJsonAsync<SupplierDto>($"/api/suppliers/{id}");
        Assert.Equal(id, supplier!.Id);
        using var missing = await client.GetAsync($"/api/suppliers/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
    [Fact]
    public async Task UC015ReturnsDeterministicPageAndRejectsInvalidPagination()
    {
        await SeedSupplierAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var page = await client.GetFromJsonAsync<PagedResult<SupplierDto>>("/api/suppliers?page=1&pageSize=2");
        await using var connection = await database.Connections.OpenAsync();
        var expected = await connection.QueryAsync<Guid>("SELECT TOP (2) Id FROM dbo.Suppliers ORDER BY Name, Id");
        Assert.Equal(expected, page!.Items.Select(item => item.Id));
        Assert.Equal(2, page.PageSize);
        Assert.Equal(await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Suppliers"), page.TotalCount);
        using var invalid = await client.GetAsync("/api/suppliers?page=1&pageSize=101");
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }
    [Fact]
    public async Task UC016UpdatesSupplierAndRejectsInvalidEmailAndMissingResource()
    {
        var id = await SeedSupplierAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var updated = await client.PutAsJsonAsync($"/api/suppliers/{id}", new SupplierInput("Updated", "new@example.test"));
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        Assert.Equal("new@example.test", (await updated.Content.ReadFromJsonAsync<SupplierDto>())!.ContactEmail);
        using var invalid = await client.PutAsJsonAsync($"/api/suppliers/{id}", new SupplierInput("Updated", "invalid"));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        using var missing = await client.PutAsJsonAsync($"/api/suppliers/{Guid.NewGuid()}", new SupplierInput("Missing", null));
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
    [Fact]
    public async Task UC017DeletesOnlyUnreferencedSupplier()
    {
        var id = await SeedSupplierAsync();
        var productId = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var referenced = await connection.ExecuteScalarAsync<Guid>("SELECT SupplierId FROM dbo.Products WHERE Id=@Id", new { Id = productId });
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var deleted = await client.DeleteAsync($"/api/suppliers/{id}");
        using var missing = await client.DeleteAsync($"/api/suppliers/{id}");
        using var blocked = await client.DeleteAsync($"/api/suppliers/{referenced}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, blocked.StatusCode);
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Products WHERE Id=@Id", new { Id = productId }));
    }
    [Theory]
    [InlineData("", null)]
    [InlineData("Supplier", "invalid")]
    public async Task UC013RejectsInvalidData(string name, string? email)
    {
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/suppliers", new SupplierInput(name, email));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    private async Task<Guid> SeedSupplierAsync()
    {
        var id = Guid.NewGuid();
        await using var connection = await database.Connections.OpenAsync();
        await connection.ExecuteAsync("INSERT dbo.Suppliers(Id,Name) VALUES(@Id,@Name)", new { Id = id, Name = $"Supplier-{id:N}" });

        return id;
    }
}
