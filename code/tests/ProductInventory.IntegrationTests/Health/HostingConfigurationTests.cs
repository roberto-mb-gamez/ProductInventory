using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductInventory.Api;
namespace ProductInventory.IntegrationTests.Health;
[Collection("HostingEnvironment")]
public sealed class HostingConfigurationTests
{
    [Fact]
    public async Task AspNetCoreHttpPortsEnvironmentVariableConfiguresHostingPort()
    {
        var previousValue = Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_HTTP_PORTS", "8080");
            await using var factory = new WebApplicationFactory<Program>();
            var configuration = factory.Services.GetRequiredService<IConfiguration>();
            Assert.Equal("8080", configuration["HTTP_PORTS"]);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_HTTP_PORTS", previousValue);
        }
    }
}
