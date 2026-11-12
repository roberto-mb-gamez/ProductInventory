using Dapper;
using ProductInventory.Application.Products;
namespace ProductInventory.Infrastructure.Persistence.Products;
public sealed class DapperProductWriter(SqlConnectionFactory connections) : IProductWriter
{
    public async Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        const string sql = """
            INSERT INTO dbo.Products (Id, Sku, Name, Description, Price, CategoryId, SupplierId)
            VALUES (@Id, @Sku, @Name, @Description, @Price, @CategoryId, @SupplierId);
            """;
        var parameters = new { Id = id, request.Sku, request.Name, request.Description, request.Price, request.CategoryId, request.SupplierId };
        await using var connection = await connections.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        return id;
    }
}
