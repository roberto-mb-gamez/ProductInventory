using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace ProductInventory.Infrastructure.Persistence;
internal sealed class SqlServerHealthCheck(SqlConnectionFactory connections) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await connections.OpenAsync(cancellationToken);
            await connection.ExecuteScalarAsync<int>(new CommandDefinition("SELECT 1", commandTimeout: 5, cancellationToken: cancellationToken));

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {

            return HealthCheckResult.Unhealthy("SQL Server is unavailable.");
        }
    }
}
