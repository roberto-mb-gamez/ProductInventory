using ProductInventory.Api.Documentation;
using ProductInventory.Api.Hosting;
using ProductInventory.Api.Security;
namespace ProductInventory.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ApiConfiguration.ConfigureServices(builder);
        var app = builder.Build();
        app.UseApiTransportSecurity();
        app.UseApiDocumentation();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health").AllowAnonymous();
        app.Run();
    }
}
