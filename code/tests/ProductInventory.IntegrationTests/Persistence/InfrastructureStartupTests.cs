using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using ProductInventory.Application.Inventory;
using ProductInventory.Application.Products;
using ProductInventory.Infrastructure;
using ProductInventory.Infrastructure.Persistence;
namespace ProductInventory.IntegrationTests.Persistence;
[Collection("SqlServer")]
public sealed class InfrastructureStartupTests(SqlServerFixture database)
{
    [Fact]
    public async Task StartupProvisionsDatabaseAndSchemaAndReportsHealthy()
    {
        var builder = new SqlConnectionStringBuilder(database.ConnectionString)
            { InitialCatalog = $"ProductInventoryStartupTests_{Guid.NewGuid():N}" };
        var services = new ServiceCollection().AddLogging();
        services.AddInfrastructure(new(builder.ConnectionString, true));
        await using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        foreach (var initializer in provider.GetServices<IHostedService>())
        {
            await initializer.StartAsync(CancellationToken.None);
            await initializer.StartAsync(CancellationToken.None);
        }
        var report = await provider.GetRequiredService<HealthCheckService>().CheckHealthAsync();
        Assert.Equal(HealthStatus.Healthy, report.Status);
        await using var connection = await provider.GetRequiredService<SqlConnectionFactory>().OpenAsync();
        Assert.Equal(4, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.tables"));
        using var scope = provider.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IProductReader>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IProductWriter>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IStockMovementWriter>());
        Console.WriteLine($"Isolated startup database: {builder.InitialCatalog}");
    }
    [Fact]
    public async Task UnavailableDatabaseReportsUnhealthyWithoutExposingConnectionDetails()
    {
        var builder = new SqlConnectionStringBuilder(database.ConnectionString)
            { InitialCatalog = $"Missing_{Guid.NewGuid():N}", ConnectTimeout = 1 };
        var services = new ServiceCollection().AddLogging();
        services.AddInfrastructure(new(builder.ConnectionString));
        await using var provider = services.BuildServiceProvider();
        var report = await provider.GetRequiredService<HealthCheckService>().CheckHealthAsync();
        Assert.Equal(HealthStatus.Unhealthy, report.Status);
        Assert.Equal("SQL Server is unavailable.", report.Entries["sqlserver"].Description);
        Assert.Null(report.Entries["sqlserver"].Exception);
    }
}
