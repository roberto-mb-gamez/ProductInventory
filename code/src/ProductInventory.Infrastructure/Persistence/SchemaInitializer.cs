using Dapper;
namespace ProductInventory.Infrastructure.Persistence;
public sealed class SchemaInitializer(SqlConnectionFactory connections)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var assembly = typeof(SchemaInitializer).Assembly;
        await using var stream = assembly.GetManifestResourceStream("ProductInventory.Infrastructure.Persistence.Schema.001_initial.sql")
            ?? throw new InvalidOperationException("The initial schema resource is missing.");
        using var reader = new StreamReader(stream);
        var sql = await reader.ReadToEndAsync(cancellationToken);
        await using var connection = await connections.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken, commandTimeout: 90));
    }
}
