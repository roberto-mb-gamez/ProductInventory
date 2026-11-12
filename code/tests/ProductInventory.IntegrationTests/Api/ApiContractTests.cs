using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Dapper;
using ProductInventory.Application.Categories;
using ProductInventory.IntegrationTests.Persistence;
namespace ProductInventory.IntegrationTests.Api;
[Collection("SqlServer")]
public sealed class ApiContractTests(SqlServerFixture database)
{
    [Fact]
    public async Task OpenApiContainsExactlySixteenBusinessOperationsAndMovementCreatedContract()
    {
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var document = JsonDocument.Parse(await client.GetStringAsync("/swagger/v1/swagger.json"));
        var paths = document.RootElement.GetProperty("paths");
        Assert.Equal(7, paths.EnumerateObject().Count());
        Assert.Equal(16, paths.EnumerateObject().Sum(path => path.Value.EnumerateObject().Count()));
        Assert.True(paths.GetProperty("/api/products/{id}/movements").GetProperty("post").GetProperty("responses").TryGetProperty("201", out _));
        Assert.True(paths.GetProperty("/api/categories").TryGetProperty("get", out _));
        Assert.True(paths.GetProperty("/api/suppliers").TryGetProperty("get", out _));
    }
    [Fact]
    public async Task MalformedJsonReturnsGenericProblemDetails()
    {
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var content = new StringContent("{\"name\":", Encoding.UTF8, "application/json");
        using var response = await client.PostAsync("/api/categories", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Exception", body);
        using var problem = JsonDocument.Parse(body);
        Assert.Equal(400, problem.RootElement.GetProperty("status").GetInt32());
    }
    [Fact]
    public async Task UC011RejectsDuplicateNameAndKeepsOriginalData()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var name = $"Unique-{first:N}";
        await using var connection = await database.Connections.OpenAsync();
        await connection.ExecuteAsync("INSERT dbo.Categories(Id,Name) VALUES(@First,@Name),(@Second,@Other)",
            new { First = first, Second = second, Name = name, Other = $"Unique-{second:N}" });
        await using var factory = new BusinessApiFactory(database.ConnectionString);
        using var client = factory.CreateClient();
        using var conflict = await client.PutAsJsonAsync($"/api/categories/{second}", new CategoryInput(name.ToLowerInvariant(), "Changed"));
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        Assert.Null(await connection.ExecuteScalarAsync<string?>("SELECT Description FROM dbo.Categories WHERE Id=@Id", new { Id = second }));
        using var unchanged = await client.PutAsJsonAsync($"/api/categories/{first}", new CategoryInput(name, "Allowed"));
        Assert.Equal(HttpStatusCode.OK, unchanged.StatusCode);
    }
}
