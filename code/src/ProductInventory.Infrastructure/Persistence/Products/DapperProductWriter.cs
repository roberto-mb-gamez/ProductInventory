using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Products;
using ProductInventory.Application.Errors;
using ProductInventory.Domain;
using ProductInventory.Domain.Products;
namespace ProductInventory.Infrastructure.Persistence.Products;
public sealed class DapperProductWriter(SqlConnectionFactory connections) : IProductWriter
{
    public async Task<ProductDto> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {

        return await SaveAsync("dbo.Product_Create", product, cancellationToken) ?? throw new InvalidOperationException("The inserted product was not returned.");
    }
    public Task<ProductDto?> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {

        return SaveAsync("dbo.Product_Update", product, cancellationToken);
    }
    private async Task<ProductDto?> SaveAsync(string procedure, Product product, CancellationToken cancellationToken)
    {
        var data = product.Data;
        var parameters = new { product.Id, data.Sku, data.Name, data.Description, data.Price, data.CategoryId, data.SupplierId };
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.QuerySingleOrDefaultAsync<ProductDto>(new CommandDefinition(procedure, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
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

            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("dbo.Product_Delete", new { Id = id }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number is 547 or 1205)
        {
            throw new ConflictException("No se puede eliminar el producto por movimientos históricos o concurrencia.");
        }
    }
}
