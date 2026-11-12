using Microsoft.Extensions.Hosting;
namespace ProductInventory.Infrastructure.Persistence;
internal sealed class DatabaseInitializationService(DatabaseProvisioner provisioner, SchemaInitializer schema) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await provisioner.EnsureDatabaseAsync(cancellationToken);
        await schema.InitializeAsync(cancellationToken);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}
