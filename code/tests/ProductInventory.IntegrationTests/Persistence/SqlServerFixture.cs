using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Infrastructure.Persistence;
using ProductInventory.Infrastructure.Persistence.Products;
using ProductInventory.Application.Products;
using Microsoft.EntityFrameworkCore;
using ProductInventory.Domain.Products;
namespace ProductInventory.IntegrationTests.Persistence;
public sealed class SqlServerFixture : IAsyncLifetime
{
    public string ConnectionString { get; private set; } = string.Empty;
    public SqlConnectionFactory Connections => new(ConnectionString);
    public InventoryReadDbContext CreateReadContext()
    {
        var options = new DbContextOptionsBuilder<InventoryReadDbContext>().UseSqlServer(ConnectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;

        return new InventoryReadDbContext(options);
    }
    public async Task<Guid> CreateProductAsync()
    {
        var categoryId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        await using var connection = await Connections.OpenAsync();
        await connection.ExecuteAsync("""
            INSERT INTO dbo.Categories (Id, Name) VALUES (@CategoryId, @CategoryName);
            INSERT INTO dbo.Suppliers (Id, Name) VALUES (@SupplierId, 'Test supplier');
            """, new { CategoryId = categoryId, CategoryName = $"Category-{categoryId:N}", SupplierId = supplierId });
        var product = new Product(Guid.NewGuid(), new($"SKU-{Guid.NewGuid():N}", "Test product", "Description", 12.3456m, categoryId, supplierId));

        return (await new DapperProductWriter(Connections).CreateAsync(product)).Id;
    }
    public async Task InitializeAsync()
    {
        var configured = Environment.GetEnvironmentVariable("PRODUCTINVENTORY_TEST_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Set PRODUCTINVENTORY_TEST_CONNECTION_STRING to a SQL Server connection with CREATE DATABASE permission.");
        var builder = new SqlConnectionStringBuilder(configured) { InitialCatalog = "master" };
        var databaseName = $"ProductInventoryTests_{Guid.NewGuid():N}";
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync($"CREATE DATABASE [{databaseName}]");
        builder.InitialCatalog = databaseName;
        ConnectionString = builder.ConnectionString;
        Console.WriteLine($"Isolated SQL Server test database: {databaseName}");
        await new SchemaInitializer(Connections).InitializeAsync();
    }
    public Task DisposeAsync()
    {
        // Preserve the isolated database for inspection; never delete a configured database.

        return Task.CompletedTask;
    }
}
