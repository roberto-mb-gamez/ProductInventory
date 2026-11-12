using Dapper;
using Microsoft.Data.SqlClient;
namespace ProductInventory.Infrastructure.Persistence;
public sealed class SchemaInitializer(SqlConnectionFactory connections)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await connections.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        var scripts = typeof(SchemaInitializer).Assembly.GetManifestResourceNames()
            .Where(name => name.Contains(".Persistence.Schema.", StringComparison.Ordinal) && name.EndsWith(".sql", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal);
        foreach (var script in scripts)
            await ApplyScriptAsync(connection, transaction, script, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
    private static async Task ApplyScriptAsync(SqlConnection connection, SqlTransaction transaction, string resourceName, CancellationToken cancellationToken)
    {
        await using var stream = typeof(SchemaInitializer).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The schema resource is missing.");
        using var reader = new StreamReader(stream);
        var sql = await reader.ReadToEndAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken, commandTimeout: 90));
    }
}
