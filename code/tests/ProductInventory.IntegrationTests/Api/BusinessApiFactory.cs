using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Api;
using ProductInventory.Infrastructure;
namespace ProductInventory.IntegrationTests.Api;
public sealed class BusinessApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Identity:Authority"] = "http://127.0.0.1:1/realms/unused", ["Identity:Audience"] = "productinventory-api",
            ["ConnectionStrings:Inventory"] = null, ["Database:Initialize"] = "false", ["Swagger:Enabled"] = "true",
            ["Swagger:ClientId"] = "business-tests"
        }));
        builder.ConfigureServices(services => ConfigureBusinessServices(services, connectionString));
    }
    private static void ConfigureBusinessServices(IServiceCollection services, string connectionString)
    {
        services.AddInfrastructure(new(connectionString));
        services.AddAuthentication(options => options.DefaultScheme = "BusinessTests")
            .AddScheme<AuthenticationSchemeOptions, BusinessTestAuthenticationHandler>("BusinessTests", _ => { });
        services.AddAuthorization(options =>
        {
            var policy = new AuthorizationPolicyBuilder("BusinessTests").RequireAuthenticatedUser().Build();
            options.DefaultPolicy = policy;
            options.FallbackPolicy = policy;
        });
    }
}
