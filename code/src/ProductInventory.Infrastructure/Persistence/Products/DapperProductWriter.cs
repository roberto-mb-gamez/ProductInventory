using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Products;
using ProductInventory.Application.Errors;
using ProductInventory.Domain;
using ProductInventory.Domain.Products;
namespace ProductInventory.Infrastructure.Persistence.Products;
public sealed class DapperProductWriter(SqlConnectionFactory connections) : IProductWriter
{
    private const string OutputColumns = " OUTPUT inserted.Id,inserted.Sku,inserted.Name,inserted.Description,inserted.Price,inserted.StockQuantity,inserted.CategoryId,inserted.SupplierId ";
    public async Task<ProductDto> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT dbo.Products(Id,Sku,Name,Description,Price,CategoryId,SupplierId)" + OutputColumns
            + "VALUES(@Id,@Sku,@Name,@Description,@Price,@CategoryId,@SupplierId);";

        return await SaveAsync(sql, product, cancellationToken) ?? throw new InvalidOperationException("The inserted product was not returned.");
    }
    public Task<ProductDto?> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Products SET Sku=@Sku,Name=@Name,Description=@Description,Price=@Price,CategoryId=@CategoryId,SupplierId=@SupplierId"
            + OutputColumns + "WHERE Id=@Id;";

        return SaveAsync(sql, product, cancellationToken);
    }
    private async Task<ProductDto?> SaveAsync(string sql, Product product, CancellationToken cancellationToken)
    {
        var data = product.Data;
        var parameters = new { product.Id, data.Sku, data.Name, data.Description, data.Price, data.CategoryId, data.SupplierId };
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.QuerySingleOrDefaultAsync<ProductDto>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number is 2601 or 2627 or 547 or 8115 or 1205)
        {
            throw TranslateSaveError(exception);
        }
    }
    private static Exception TranslateSaveError(SqlException exception)
    {

        return exception.Number switch
        {
            547 => new NotFoundException("La categoría o el proveedor no existe."),
            8115 => new ValidationException("El precio está fuera del rango admitido."),
            _ => new ConflictException("El SKU ya existe o se produjo un conflicto de concurrencia.")
        };
    }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.ExecuteAsync(new CommandDefinition("DELETE dbo.Products WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken)) != 0;
        }
        catch (SqlException exception) when (exception.Number is 547 or 1205)
        {
            throw new ConflictException("No se puede eliminar el producto por movimientos históricos o concurrencia.");
        }
    }
}
