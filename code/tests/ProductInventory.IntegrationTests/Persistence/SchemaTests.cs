using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Infrastructure.Persistence;
namespace ProductInventory.IntegrationTests.Persistence;
[Collection("SqlServer")]
public sealed class SchemaTests(SqlServerFixture database)
{
    public static TheoryData<MovementConstraintCase> InvalidMovementCases => new()
    {
        new("IN", 0, 0, 0, 0),
        new("BAD", 1, 0, 1, 0),
        new("in", 1, 0, 1, 0),
        new("OUT", 1, 0, -1, 0),
        new("IN", 1, 0, 3, 0),
        new("IN", 1, 0, 1, 60)
    };
    [Fact]
    public async Task InitializationCreatesExactlyFourTablesAndIsRepeatable()
    {
        await new SchemaInitializer(database.Connections).InitializeAsync();
        await using var connection = await database.Connections.OpenAsync();
        var tables = await connection.QueryAsync<string>("SELECT name FROM sys.tables ORDER BY name");
        Assert.Equal(["Categories", "InventoryMovements", "Products", "Suppliers"], tables);
    }
    [Theory]
    [MemberData(nameof(InvalidMovementCases))]
    public async Task SqlRejectsInvalidMovementHistory(MovementConstraintCase testCase)
    {
        var id = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        const string sql = """
            INSERT INTO dbo.InventoryMovements (Id, ProductId, Type, Quantity, OccurredAt, StockBefore, StockAfter)
            VALUES (NEWID(), @ProductId, @Type, @Quantity, @OccurredAt, @StockBefore, @StockAfter);
            """;
        var parameters = new { ProductId = id, testCase.Type, testCase.Quantity, testCase.StockBefore,
            testCase.StockAfter, OccurredAt = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromMinutes(testCase.OffsetMinutes)) };
        var error = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(sql, parameters));
        Assert.Equal(547, error.Number);
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.InventoryMovements WHERE ProductId = @Id", new { Id = id }));
    }
}
