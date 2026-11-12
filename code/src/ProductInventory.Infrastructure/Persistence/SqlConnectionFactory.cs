using Microsoft.Data.SqlClient;
namespace ProductInventory.Infrastructure.Persistence;
public sealed class SqlConnectionFactory(string connectionString)
{
    public async Task<SqlConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);

            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}
