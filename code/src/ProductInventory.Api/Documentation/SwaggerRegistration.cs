using ProductInventory.Api.Security;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace ProductInventory.Api.Documentation;
public static class SwaggerRegistration
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddOptions<SwaggerGenOptions>().Configure<IConfiguration, IWebHostEnvironment>(SwaggerConfiguration.Configure);

        return services;
    }
    public static void UseApiDocumentation(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Swagger:Enabled"))
        {

            return;
        }
        var clientId = IdentitySettings.GetRequired(app.Configuration, "Swagger:ClientId");
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductInventory API v1");
            options.OAuthClientId(clientId);
            options.OAuthScopes("openid", "profile");
            options.OAuthUsePkce();
        });
    }
}
