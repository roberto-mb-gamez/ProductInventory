using Dapper;
using Microsoft.Data.SqlClient;
namespace ProductInventory.Infrastructure.Persistence;
internal sealed class DatabaseProvisioner(SqlServerSettings settings)
{
    private const string ProvisionSql = """
        DECLARE @result int;
        EXEC @result = sys.sp_getapplock @Resource = @LockName, @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = 60000;
        IF @result < 0 THROW 51000, 'Unable to acquire database initialization lock.', 1;
        BEGIN TRY
            IF DB_ID(@DatabaseName) IS NULL EXEC sys.sp_executesql @CreateStatement;
        END TRY
        BEGIN CATCH
            EXEC sys.sp_releaseapplock @Resource = @LockName, @LockOwner = 'Session';
            THROW;
        END CATCH;
        EXEC sys.sp_releaseapplock @Resource = @LockName, @LockOwner = 'Session';
        """;
    public async Task EnsureDatabaseAsync(CancellationToken cancellationToken)
    {
        var builder = new SqlConnectionStringBuilder(settings.ConnectionString);
        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName) || new[] { "master", "model", "msdb", "tempdb" }.Contains(databaseName, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Initialization requires a dedicated application database.");
        builder.InitialCatalog = "master";
        builder.Pooling = false;
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        using var commandBuilder = new SqlCommandBuilder();
        var createStatement = $"CREATE DATABASE {commandBuilder.QuoteIdentifier(databaseName)}";
        var parameters = new { DatabaseName = databaseName, CreateStatement = createStatement, LockName = $"ProductInventory.Create.{databaseName}" };
        await connection.ExecuteAsync(new CommandDefinition(ProvisionSql, parameters, cancellationToken: cancellationToken, commandTimeout: 90));
    }
}
