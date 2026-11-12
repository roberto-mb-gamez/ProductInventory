using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Inventory;
using ProductInventory.Infrastructure.Persistence;
namespace ProductInventory.IntegrationTests.Persistence;
[Collection("SqlServer")]
public sealed class StoredProcedureTests(SqlServerFixture database)
{
    [Fact]
    public async Task InitializationInstallsWriteProceduresAndPreservesExistingData()
    {
        var connections = await CreateLegacyDatabaseAsync();
        await using var connection = await connections.OpenAsync();
        var id = Guid.NewGuid();
        await connection.ExecuteAsync("INSERT dbo.Categories(Id,Name) VALUES(@Id,'Existing category')", new { Id = id });
        await new SchemaInitializer(connections).InitializeAsync();
        await new SchemaInitializer(connections).InitializeAsync();
        var procedures = await connection.QueryAsync<string>("SELECT name FROM sys.procedures WHERE schema_id = SCHEMA_ID('dbo')");
        Assert.Equal(10, procedures.Count());
        Assert.Contains("InventoryMovement_Register", procedures);
        Assert.Equal("Existing category", await connection.ExecuteScalarAsync<string>("SELECT Name FROM dbo.Categories WHERE Id=@Id", new { Id = id }));
    }
    [Fact]
    public async Task DirectMovementProcedureRollsBackFailedInsertAndClosesItsTransaction()
    {
        var productId = await database.CreateProductAsync();
        var movementId = Guid.NewGuid();
        await using var connection = await database.Connections.OpenAsync();
        var parameters = CreateMovementParameters(productId, movementId);
        var movement = await connection.QuerySingleAsync<StockMovementDto>("dbo.InventoryMovement_Register", parameters, commandType: CommandType.StoredProcedure);
        Assert.Equal(MovementWriteStatus.Created, parameters.Get<MovementWriteStatus>("Status"));
        Assert.Equal(1, movement.StockAfter);
        await Assert.ThrowsAsync<SqlException>(() => connection.QuerySingleAsync<StockMovementDto>("dbo.InventoryMovement_Register", CreateMovementParameters(productId, movementId), commandType: CommandType.StoredProcedure));
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT @@TRANCOUNT"));
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT StockQuantity FROM dbo.Products WHERE Id=@Id", new { Id = productId }));
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.InventoryMovements WHERE ProductId=@Id", new { Id = productId }));
    }
    private static DynamicParameters CreateMovementParameters(Guid productId, Guid movementId)
    {
        var parameters = new DynamicParameters(new { Id = movementId, ProductId = productId, Type = 0, Quantity = 1 });
        parameters.Add("Status", dbType: DbType.Int32, direction: ParameterDirection.Output);

        return parameters;
    }
    [Fact]
    public async Task ProcedureAllowsExecutionWithoutDirectTablePermissions()
    {
        await using var connection = await database.Connections.OpenAsync();
        var user = $"ProcedureCaller_{Guid.NewGuid():N}";
        await connection.ExecuteAsync($"CREATE USER [{user}] WITHOUT LOGIN; GRANT EXECUTE ON OBJECT::dbo.Category_Create TO [{user}];");
        try
        {
            await connection.ExecuteAsync($"EXECUTE AS USER = '{user}';");
            var id = Guid.NewGuid();
            var category = await connection.QuerySingleAsync<CategoryDto>("dbo.Category_Create", new { Id = id, Name = $"Procedure-{id:N}", Description = (string?)null }, commandType: CommandType.StoredProcedure);
            Assert.Equal(id, category.Id);
            var error = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Categories"));
            Assert.Equal(229, error.Number);
        }
        finally
        {
            await connection.ExecuteAsync("REVERT;");
        }
    }
    private async Task<SqlConnectionFactory> CreateLegacyDatabaseAsync()
    {
        var name = $"ProductInventoryLegacyTests_{Guid.NewGuid():N}";
        await using var existing = await database.Connections.OpenAsync();
        await existing.ExecuteAsync($"CREATE DATABASE [{name}]");
        var settings = new SqlConnectionStringBuilder(database.ConnectionString) { InitialCatalog = name };
        var connections = new SqlConnectionFactory(settings.ConnectionString);
        await using var connection = await connections.OpenAsync();
        await using var stream = typeof(SchemaInitializer).Assembly.GetManifestResourceStream("ProductInventory.Infrastructure.Persistence.Schema.001_initial.sql")!;
        using var reader = new StreamReader(stream);
        await connection.ExecuteAsync(await reader.ReadToEndAsync());

        return connections;
    }
}
