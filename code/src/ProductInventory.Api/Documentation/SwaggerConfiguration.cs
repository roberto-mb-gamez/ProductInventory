using Microsoft.OpenApi.Models;
using ProductInventory.Api.Security;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace ProductInventory.Api.Documentation;
internal static class SwaggerConfiguration
{
    public static void Configure(SwaggerGenOptions options, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var settings = IdentitySettings.Read(configuration, environment);
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductInventory API", Version = "v1" });
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{settings.Authority}/protocol/openid-connect/auth"),
                    TokenUrl = new Uri($"{settings.Authority}/protocol/openid-connect/token"),
                    Scopes = new Dictionary<string, string> { ["openid"] = "OpenID Connect", ["profile"] = "User profile" }
                }
            }
        });
        options.OperationFilter<AuthorizationOperationFilter>();
    }
}
