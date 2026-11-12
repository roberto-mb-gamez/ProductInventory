using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Errors;
using ProductInventory.Domain.Suppliers;
namespace ProductInventory.Infrastructure.Persistence.Suppliers;
public sealed class DapperSupplierWriter(SqlConnectionFactory connections) : ISupplierWriter
{
    public async Task<SupplierDto> CreateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {

        return await SaveAsync("dbo.Supplier_Create", supplier, cancellationToken) ?? throw new InvalidOperationException("The inserted supplier was not returned.");
    }
    public Task<SupplierDto?> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {

        return SaveAsync("dbo.Supplier_Update", supplier, cancellationToken);
    }
    private async Task<SupplierDto?> SaveAsync(string procedure, Supplier supplier, CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.QuerySingleOrDefaultAsync<SupplierDto>(new CommandDefinition(procedure, supplier, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
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

            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("dbo.Supplier_Delete", new { Id = id }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number is 547 or 1205)
        {
            throw new ConflictException("No se puede eliminar el proveedor por productos asociados o concurrencia.");
        }
    }
}
