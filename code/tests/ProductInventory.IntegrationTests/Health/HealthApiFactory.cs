using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ProductInventory.Api;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductInventory.Application.Categories;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Products;
using ProductInventory.Application.Inventory;
namespace ProductInventory.IntegrationTests.Health;
public sealed class HealthApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Identity:Authority"] = "http://127.0.0.1:1/realms/unused",
            ["Identity:Audience"] = "productinventory-api",
            ["ConnectionStrings:Inventory"] = null,
            ["Database:Initialize"] = "false",
            ["Swagger:Enabled"] = "false"
        }));
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(Mock.Of<ICategoryReader>());
            services.AddSingleton(Mock.Of<ICategoryWriter>());
            services.AddSingleton(Mock.Of<ISupplierReader>());
            services.AddSingleton(Mock.Of<ISupplierWriter>());
            services.AddSingleton(Mock.Of<IProductReader>());
            services.AddSingleton(Mock.Of<IProductWriter>());
            services.AddSingleton(Mock.Of<IStockMovementWriter>());
        });
    }
}
