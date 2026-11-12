using ProductInventory.Infrastructure;
using ProductInventory.Infrastructure.Persistence;
namespace ProductInventory.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddEnvironmentVariables("ASPNETCORE_");
        builder.Services.AddHealthChecks();
        var connectionString = builder.Configuration.GetConnectionString("Inventory");
        var initializeDatabase = builder.Configuration.GetValue<bool>("Database:Initialize");
        if (initializeDatabase && string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database initialization requires ConnectionStrings__Inventory.");
        if (!string.IsNullOrWhiteSpace(connectionString))
            builder.Services.AddInfrastructure(new SqlServerSettings(connectionString, initializeDatabase));
        var app = builder.Build();
        app.MapHealthChecks("/health");
        app.Run();
    }
}
