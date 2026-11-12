using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Dapper;
using ProductInventory.Application.Inventory;
using ProductInventory.IntegrationTests.Persistence;
namespace ProductInventory.IntegrationTests.Api;
[Collection("SqlServer")]
public sealed class MovementApiTests(SqlServerFixture database)
{
    [Fact]
    public async Task UC006RegistersInAndOutWithCreatedAndUtcBalances()
    {
        var id = await database.CreateProductAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var incoming = await client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput("IN", 8));
        using var outgoing = await client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput("OUT", 3));
        Assert.Equal(HttpStatusCode.Created, incoming.StatusCode);
        Assert.Equal(HttpStatusCode.Created, outgoing.StatusCode);
        using var document = JsonDocument.Parse(await outgoing.Content.ReadAsStringAsync());
        Assert.Equal("OUT", document.RootElement.GetProperty("type").GetString());
        Assert.Equal(8, document.RootElement.GetProperty("stockBefore").GetInt32());
        Assert.Equal(5, document.RootElement.GetProperty("stockAfter").GetInt32());
        Assert.Equal(TimeSpan.Zero, document.RootElement.GetProperty("occurredAt").GetDateTimeOffset().Offset);
        await AssertStateAsync(id, 5, 2);
    }
    [Theory]
    [InlineData("BAD", 1)]
    [InlineData("IN", 0)]
    [InlineData("OUT", -1)]
    public async Task UC006RejectsInvalidMovementWithoutEffects(string type, int quantity)
    {
        var id = await database.CreateProductAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput(type, quantity));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task UC006RejectsMissingProductAndInsufficientStock()
    {
        var id = await database.CreateProductAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var missing = await client.PostAsJsonAsync($"/api/products/{Guid.NewGuid()}/movements", new StockMovementInput("IN", 1));
        using var insufficient = await client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput("OUT", 1));
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, insufficient.StatusCode);
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task UC006RollsBackOnInsertFailureAndHidesInternalErrors()
    {
        var id = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        await connection.ExecuteAsync($"""
            CREATE TRIGGER dbo.[TR_ApiFailure_{id:N}] ON dbo.InventoryMovements AFTER INSERT AS
            IF EXISTS (SELECT 1 FROM inserted WHERE ProductId='{id}') THROW 51002, 'Injected internal insert failure.', 1;
            """);
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput("IN", 2));
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.DoesNotContain("Injected", await response.Content.ReadAsStringAsync());
        await AssertStateAsync(id, 0, 0);
    }
    [Fact]
    public async Task UC006ConcurrentOutputsCannotOversell()
    {
        var id = await database.CreateProductAsync();
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var incoming = await client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput("IN", 10));
        Assert.Equal(HttpStatusCode.Created, incoming.StatusCode);
        var tasks = Enumerable.Range(0, 20).Select(_ => client.PostAsJsonAsync($"/api/products/{id}/movements", new StockMovementInput("OUT", 1)));
        var responses = await Task.WhenAll(tasks);
        Assert.Equal(10, responses.Count(response => response.StatusCode == HttpStatusCode.Created));
        Assert.Equal(10, responses.Count(response => response.StatusCode == HttpStatusCode.Conflict));
        foreach (var response in responses)
            response.Dispose();
        await AssertStateAsync(id, 0, 11);
    }
    private async Task AssertStateAsync(Guid id, int stock, int movements)
    {
        await using var connection = await database.Connections.OpenAsync();
        Assert.Equal(stock, await connection.ExecuteScalarAsync<int>("SELECT StockQuantity FROM dbo.Products WHERE Id=@Id", new { Id = id }));
        Assert.Equal(movements, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.InventoryMovements WHERE ProductId=@Id", new { Id = id }));
    }
}
