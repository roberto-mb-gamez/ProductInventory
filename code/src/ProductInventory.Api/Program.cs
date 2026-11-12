using ProductInventory.Api.Documentation;
using ProductInventory.Api.Hosting;
using ProductInventory.Api.Security;
using ProductInventory.Api.Features.Categories;
using ProductInventory.Api.Features.Suppliers;
using ProductInventory.Api.Features.Products;
using ProductInventory.Api.Features.Movements;
namespace ProductInventory.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ApiConfiguration.ConfigureServices(builder);
        var app = builder.Build();
        app.UseExceptionHandler();
        app.UseApiTransportSecurity();
        app.UseApiDocumentation();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapCategoryEndpoints();
        app.MapSupplierEndpoints();
        app.MapProductEndpoints();
        app.MapMovementEndpoints();
        app.Run();
    }
}
