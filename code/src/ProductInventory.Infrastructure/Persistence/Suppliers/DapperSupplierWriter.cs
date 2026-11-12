using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Errors;
using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Infrastructure.Persistence.Suppliers;
public sealed class DapperSupplierWriter(SqlConnectionFactory connections) : ISupplierWriter
{
    public async Task<SupplierDto> CreateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT dbo.Suppliers(Id,Name,ContactEmail) OUTPUT inserted.Id,inserted.Name,inserted.ContactEmail VALUES(@Id,@Name,@ContactEmail);";

        return await SaveAsync(sql, supplier, cancellationToken) ?? throw new InvalidOperationException("The inserted supplier was not returned.");
    }
    public Task<SupplierDto?> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Suppliers SET Name=@Name,ContactEmail=@ContactEmail OUTPUT inserted.Id,inserted.Name,inserted.ContactEmail WHERE Id=@Id;";

        return SaveAsync(sql, supplier, cancellationToken);
    }
    private async Task<SupplierDto?> SaveAsync(string sql, Supplier supplier, CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.QuerySingleOrDefaultAsync<SupplierDto>(new CommandDefinition(sql, supplier, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number == 1205)
        {
            throw new ConflictException("Se produjo un conflicto de concurrencia al guardar el proveedor.");
        }
    }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.ExecuteAsync(new CommandDefinition("DELETE dbo.Suppliers WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken)) != 0;
        }
        catch (SqlException exception) when (exception.Number is 547 or 1205)
        {
            throw new ConflictException("No se puede eliminar el proveedor por productos asociados o concurrencia.");
        }
    }
}
