using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Products;
using ProductInventory.Infrastructure.Persistence.Products;
namespace ProductInventory.IntegrationTests.Persistence;
[Collection("SqlServer")]
public sealed class ProductPersistenceTests(SqlServerFixture database)
{
    [Fact]
    public async Task DapperCreatesZeroStockAndEfProjectsBothRelationsWithoutTracking()
    {
        var id = await database.CreateProductAsync();
        await using var context = database.CreateReadContext();
        var product = await new EfProductReader(context).GetByIdAsync(id);
        Assert.NotNull(product);
        Assert.Equal(0, product.StockQuantity);
        Assert.Equal(12.3456m, product.Price);
        Assert.StartsWith("Category-", product.CategoryName);
        Assert.Equal("Test supplier", product.SupplierName);
        Assert.Empty(context.ChangeTracker.Entries());
        Assert.Null(await new EfProductReader(context).GetByIdAsync(Guid.NewGuid()));
        await Assert.ThrowsAsync<NotSupportedException>(() => context.SaveChangesAsync());
    }
    [Fact]
    public async Task SqlRejectsCaseInsensitiveDuplicateSkuAndMissingRelations()
    {
        var id = await database.CreateProductAsync();
        await using var context = database.CreateReadContext();
        var product = await new EfProductReader(context).GetByIdAsync(id);
        Assert.NotNull(product);
        var duplicate = new CreateProductRequest(product.Sku.ToLowerInvariant(), "Duplicate", null, 1, product.CategoryId, product.SupplierId);
        var writer = new DapperProductWriter(database.Connections);
        var duplicateError = await Assert.ThrowsAsync<SqlException>(() => writer.CreateAsync(duplicate));
        Assert.Contains(duplicateError.Number, new[] { 2601, 2627 });
        var missingReference = duplicate with { Sku = $"SKU-{Guid.NewGuid():N}", CategoryId = Guid.NewGuid() };
        var referenceError = await Assert.ThrowsAsync<SqlException>(() => writer.CreateAsync(missingReference));
        Assert.Equal(547, referenceError.Number);
    }
    [Fact]
    public async Task ForeignKeysBlockDeletionOfReferencedCategoryAndSupplier()
    {
        var id = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var categoryError = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(
            "DELETE FROM dbo.Categories WHERE Id = (SELECT CategoryId FROM dbo.Products WHERE Id = @Id)", new { Id = id }));
        var supplierError = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(
            "DELETE FROM dbo.Suppliers WHERE Id = (SELECT SupplierId FROM dbo.Products WHERE Id = @Id)", new { Id = id }));
        Assert.Equal(547, categoryError.Number);
        Assert.Equal(547, supplierError.Number);
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Products WHERE Id = @Id", new { Id = id }));
    }
    [Fact]
    public async Task CategoryNamesAreUniqueIgnoringCaseButSupplierNamesAreNotUnique()
    {
        await using var connection = await database.Connections.OpenAsync();
        var name = $"Category-{Guid.NewGuid():N}";
        await connection.ExecuteAsync("INSERT INTO dbo.Categories (Id, Name) VALUES (@Id, @Name)", new { Id = Guid.NewGuid(), Name = name });
        var error = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(
            "INSERT INTO dbo.Categories (Id, Name) VALUES (@Id, @Name)", new { Id = Guid.NewGuid(), Name = name.ToLowerInvariant() }));
        Assert.Contains(error.Number, new[] { 2601, 2627 });
        await connection.ExecuteAsync("INSERT INTO dbo.Suppliers (Id, Name) VALUES (NEWID(), @Name), (NEWID(), @Name)", new { Name = name });
        Assert.Equal(2, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Suppliers WHERE Name = @Name", new { Name = name }));
    }
    [Theory]
    [InlineData("UPDATE dbo.Products SET Price = -1 WHERE Id = @Id")]
    [InlineData("UPDATE dbo.Products SET StockQuantity = -1 WHERE Id = @Id")]
    [InlineData("UPDATE dbo.Products SET Name = ' ' WHERE Id = @Id")]
    public async Task SqlRejectsInvalidProductValues(string sql)
    {
        var id = await database.CreateProductAsync();
        await using var connection = await database.Connections.OpenAsync();
        var error = await Assert.ThrowsAsync<SqlException>(() => connection.ExecuteAsync(sql, new { Id = id }));
        Assert.Equal(547, error.Number);
    }
}
