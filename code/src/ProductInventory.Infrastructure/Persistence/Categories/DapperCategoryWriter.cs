using Dapper;
using Microsoft.Data.SqlClient;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Errors;
using ProductInventory.Domain.Categories;
namespace ProductInventory.Infrastructure.Persistence.Categories;
public sealed class DapperCategoryWriter(SqlConnectionFactory connections) : ICategoryWriter
{
    public async Task<CategoryDto> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT dbo.Categories(Id,Name,Description) OUTPUT inserted.Id,inserted.Name,inserted.Description VALUES(@Id,@Name,@Description);";

        return await SaveAsync(sql, category, cancellationToken) ?? throw new InvalidOperationException("The inserted category was not returned.");
    }
    public Task<CategoryDto?> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Categories SET Name=@Name,Description=@Description OUTPUT inserted.Id,inserted.Name,inserted.Description WHERE Id=@Id;";

        return SaveAsync(sql, category, cancellationToken);
    }
    private async Task<CategoryDto?> SaveAsync(string sql, Category category, CancellationToken cancellationToken)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        try
        {

            return await connection.QuerySingleOrDefaultAsync<CategoryDto>(new CommandDefinition(sql, category, cancellationToken: cancellationToken));
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

            return await connection.ExecuteAsync(new CommandDefinition("DELETE dbo.Categories WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken)) != 0;
        }
        catch (SqlException exception) when (exception.Number is 547 or 1205)
        {
            throw new ConflictException("No se puede eliminar la categoría por productos asociados o concurrencia.");
        }
    }
}
