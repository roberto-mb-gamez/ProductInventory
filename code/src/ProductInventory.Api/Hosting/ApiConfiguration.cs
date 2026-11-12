using ProductInventory.Api.Documentation;
using ProductInventory.Api.Security;
using ProductInventory.Api.Errors;
using Microsoft.AspNetCore.Routing;
using ProductInventory.Infrastructure;
using ProductInventory.Infrastructure.Persistence;
namespace ProductInventory.Api.Hosting;
public static class ApiConfiguration
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddEnvironmentVariables("ASPNETCORE_");
        builder.Services.AddHealthChecks();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ApiExceptionHandler>();
        builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
        builder.Services.AddApplicationHandlers();
        builder.Services.AddApiSecurity();
        builder.Services.AddApiDocumentation();
        ConfigurePersistence(builder);
    }
    private static void ConfigurePersistence(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Inventory");
        var initializeDatabase = builder.Configuration.GetValue<bool>("Database:Initialize");
        if (initializeDatabase && string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database initialization requires ConnectionStrings__Inventory.");
        if (!string.IsNullOrWhiteSpace(connectionString))
            builder.Services.AddInfrastructure(new SqlServerSettings(connectionString, initializeDatabase));
    }
}
