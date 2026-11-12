using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Application.Inventory;
using ProductInventory.Application.Products;
using ProductInventory.Infrastructure.Persistence;
using ProductInventory.Infrastructure.Persistence.Inventory;
using ProductInventory.Infrastructure.Persistence.Products;
namespace ProductInventory.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, SqlServerSettings settings)
    {
        services.AddSingleton(settings);
        services.AddSingleton(new SqlConnectionFactory(settings.ConnectionString));
        services.AddSingleton<SchemaInitializer>();
        services.AddDbContext<InventoryReadDbContext>(options => options.UseSqlServer(settings.ConnectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        services.AddScoped<IProductReader, EfProductReader>();
        services.AddScoped<IProductWriter, DapperProductWriter>();
        services.AddScoped<IStockMovementWriter, DapperStockMovementWriter>();
        services.AddHealthChecks().AddCheck<SqlServerHealthCheck>("sqlserver");
        if (settings.InitializeDatabase)
            AddInitialization(services);

        return services;
    }
    private static void AddInitialization(IServiceCollection services)
    {
        services.AddSingleton<DatabaseProvisioner>();
        services.AddHostedService<DatabaseInitializationService>();
    }
}
