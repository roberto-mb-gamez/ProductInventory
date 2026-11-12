using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Errors;
using ProductInventory.Domain.Categories;
namespace ProductInventory.Infrastructure.Persistence.Categories;
public sealed class DapperCategoryWriter(SqlConnectionFactory connections) : ICategoryWriter
{
    public async Task<CategoryDto> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {

        return await SaveAsync("dbo.Category_Create", category, cancellationToken) ?? throw new InvalidOperationException("The inserted category was not returned.");
    }
    public Task<CategoryDto?> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {

        return SaveAsync("dbo.Category_Update", category, cancellationToken);
    }
    private async Task<CategoryDto?> SaveAsync(string procedure, Category category, CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.QuerySingleOrDefaultAsync<CategoryDto>(new CommandDefinition(procedure, category, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number is 2601 or 2627 or 1205)
        {
            throw new ConflictException("El nombre de categoría ya existe o se produjo un conflicto de concurrencia.");
        }
    }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("dbo.Category_Delete", new { Id = id }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        }
        catch (SqlException exception) when (exception.Number is 547 or 1205)
        {
            throw new ConflictException("No se puede eliminar la categoría por productos asociados o concurrencia.");
        }
    }
}
