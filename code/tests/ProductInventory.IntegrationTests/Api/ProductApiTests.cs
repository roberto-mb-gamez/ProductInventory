using System.Net;
using System.Net.Http.Json;
using Dapper;
using ProductInventory.Application.Products;
using ProductInventory.Application.Pagination;
using ProductInventory.Domain.Inventory;
using ProductInventory.Infrastructure.Persistence.Inventory;
using ProductInventory.IntegrationTests.Persistence;
namespace ProductInventory.IntegrationTests.Api;
[Collection("SqlServer")]
public sealed class ProductApiTests(SqlServerFixture database)
{
    [Fact]
    public async Task UC001CreatesZeroStockAndRejectsDuplicateSkuIgnoringCase()
    {
        var input = await NewInputAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var created = await client.PostAsJsonAsync("/api/products", input);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var product = await created.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(0, product!.StockQuantity);
        Assert.Equal(input.CategoryId, product.CategoryId);
        using var duplicate = await client.PostAsJsonAsync("/api/products", input with { Sku = input.Sku.ToLowerInvariant() });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }
    [Fact]
    public async Task UC001RejectsInvalidInputAndMissingReferences()
    {
        var input = await NewInputAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var invalid = await client.PostAsJsonAsync("/api/products", input with { Price = -1 });
        using var absent = await client.PostAsJsonAsync("/api/products", input with { SupplierId = Guid.Empty });
        using var missingCategory = await client.PostAsJsonAsync("/api/products", input with { CategoryId = Guid.NewGuid() });
        using var missingSupplier = await client.PostAsJsonAsync("/api/products", input with { SupplierId = Guid.NewGuid() });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, absent.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingCategory.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingSupplier.StatusCode);
    }
    [Fact]
    public async Task UC002ProjectsCategoryAndSupplierAndReturnsNotFound()
    {
        var id = await database.CreateProductAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var product = await client.GetFromJsonAsync<ProductDetailsDto>($"/api/products/{id}");
        Assert.Equal(id, product!.Id);
        Assert.StartsWith("Category-", product.CategoryName);
        Assert.Equal("Test supplier", product.SupplierName);
        using var missing = await client.GetAsync($"/api/products/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }
    [Fact]
    public async Task UC003CombinesFiltersAndReturnsPaginationMetadata()
    {
        var stocked = await database.CreateProductAsync();
        await new DapperStockMovementWriter(database.Connections).RegisterAsync(new(stocked, MovementType.In, 5));
        await using var connection = await database.Connections.OpenAsync();
        var references = await connection.QuerySingleAsync<ProductDto>("SELECT * FROM dbo.Products WHERE Id=@Id", new { Id = stocked });
        var zero = Guid.NewGuid();
        await connection.ExecuteAsync("INSERT dbo.Products(Id,Sku,Name,Price,CategoryId,SupplierId) VALUES(@Id,@Sku,'Zero',0,@CategoryId,@SupplierId)",
            new { Id = zero, Sku = $"SKU-{zero:N}", references.CategoryId, references.SupplierId });
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        var filter = $"categoryId={references.CategoryId}&supplierId={references.SupplierId}&maxStock=0";
        var page = await client.GetFromJsonAsync<PagedResult<ProductDetailsDto>>($"/api/products?{filter}");
        Assert.Equal(zero, Assert.Single(page!.Items).Id);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(1, page.Page);
        Assert.Equal(20, page.PageSize);
        var empty = await client.GetFromJsonAsync<PagedResult<ProductDetailsDto>>($"/api/products?categoryId={Guid.NewGuid()}");
        Assert.Empty(empty!.Items);
        Assert.Equal(0, empty.TotalCount);
    }
    [Theory]
    [InlineData("page=0")]
    [InlineData("pageSize=101")]
    [InlineData("maxStock=-1")]
    public async Task UC003RejectsInvalidPaginationAndFilters(string query)
    {
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.GetAsync($"/api/products?{query}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task UC004UpdatesRelationsWithoutChangingStock()
    {
        var id = await database.CreateProductAsync();
        await new DapperStockMovementWriter(database.Connections).RegisterAsync(new(id, MovementType.In, 7));
        var input = await NewInputAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.PutAsJsonAsync($"/api/products/{id}", new { input.Sku, input.Name,
            input.Description, input.Price, input.CategoryId, input.SupplierId, StockQuantity = 999 });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(7, product!.StockQuantity);
        Assert.Equal(input.CategoryId, product.CategoryId);
        Assert.Equal(input.SupplierId, product.SupplierId);
    }
    [Fact]
    public async Task UC004RejectsDuplicateSkuMissingResourceAndInvalidReference()
    {
        var first = await database.CreateProductAsync();
        var second = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var original = await connection.QuerySingleAsync<ProductDto>("SELECT * FROM dbo.Products WHERE Id=@Id", new { Id = first });
        var input = new CreateProductRequest(original.Sku, "Updated", null, 1, original.CategoryId, original.SupplierId);
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var duplicate = await client.PutAsJsonAsync($"/api/products/{second}", input);
        using var missing = await client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", input);
        using var badReference = await client.PutAsJsonAsync($"/api/products/{first}", input with { SupplierId = Guid.NewGuid() });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, badReference.StatusCode);
    }
    [Fact]
    public async Task UC005DeletesOnlyProductWithoutHistory()
    {
        var id = await database.CreateProductAsync();
        var historic = await database.CreateProductAsync();
        await new DapperStockMovementWriter(database.Connections).RegisterAsync(new(historic, MovementType.In, 1));
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var deleted = await client.DeleteAsync($"/api/products/{id}");
        using var missing = await client.DeleteAsync($"/api/products/{id}");
        using var blocked = await client.DeleteAsync($"/api/products/{historic}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, blocked.StatusCode);
        await using var connection = await database.Connections.OpenAsync();
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.InventoryMovements WHERE ProductId=@Id", new { Id = historic }));
    }
    private async Task<CreateProductRequest> NewInputAsync()
    {
        var categoryId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        await using var connection = await database.Connections.OpenAsync();
        await connection.ExecuteAsync("INSERT dbo.Categories(Id,Name) VALUES(@CategoryId,@Name); INSERT dbo.Suppliers(Id,Name) VALUES(@SupplierId,'Supplier');",
            new { CategoryId = categoryId, SupplierId = supplierId, Name = $"Category-{categoryId:N}" });

        return new($"SKU-{Guid.NewGuid():N}", "Product", null, 10, categoryId, supplierId);
    }
    [Theory]
    [InlineData("1000000000000000")]
    [InlineData("79228162514264337593543950335")]
    public async Task UC001AndUC004RejectPriceOverflowWithoutChangingData(string price)
    {
        var id = await database.CreateProductAsync();
        var input = (await NewInputAsync()) with { Price = decimal.Parse(price, System.Globalization.CultureInfo.InvariantCulture) };
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var creation = await client.PostAsJsonAsync("/api/products", input);
        using var update = await client.PutAsJsonAsync($"/api/products/{id}", input);
        Assert.Equal(HttpStatusCode.BadRequest, creation.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, update.StatusCode);
        await using var connection = await database.Connections.OpenAsync();
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Products WHERE Sku=@Sku", new { input.Sku }));
        Assert.Equal(12.3456m, await connection.ExecuteScalarAsync<decimal>("SELECT Price FROM dbo.Products WHERE Id=@Id", new { Id = id }));
    }
}
