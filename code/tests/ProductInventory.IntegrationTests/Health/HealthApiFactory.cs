using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ProductInventory.Api;
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
    }
}
