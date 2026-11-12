using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Application.Inventory;
using ProductInventory.Application.Products;
using ProductInventory.Infrastructure.Persistence;
using ProductInventory.Infrastructure.Persistence.Inventory;
using ProductInventory.Infrastructure.Persistence.Products;
using ProductInventory.Application.Categories;
using ProductInventory.Infrastructure.Persistence.Categories;
using ProductInventory.Application.Suppliers;
using ProductInventory.Infrastructure.Persistence.Suppliers;
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
        services.AddScoped<ICategoryReader, EfCategoryReader>();
        services.AddScoped<ICategoryWriter, DapperCategoryWriter>();
        services.AddScoped<ISupplierReader, EfSupplierReader>();
        services.AddScoped<ISupplierWriter, DapperSupplierWriter>();
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
