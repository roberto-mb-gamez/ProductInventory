using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
namespace ProductInventory.Api.Security;
public static class SecurityRegistration
{
    public static IServiceCollection AddApiSecurity(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IConfiguration, IWebHostEnvironment>(JwtConfiguration.Configure).ValidateOnStart();
        services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());

        return services;
    }
}
