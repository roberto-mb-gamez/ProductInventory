using System.Net;
namespace ProductInventory.IntegrationTests.Health;
public class HealthEndpointTests
{
    [Fact]
    public async Task GetHealthReturnsHealthyWithoutExternalServices()
    {
        await using var factory = new HealthApiFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
