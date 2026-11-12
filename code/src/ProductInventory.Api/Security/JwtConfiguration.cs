using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
namespace ProductInventory.Api.Security;
internal static class JwtConfiguration
{
    public static void Configure(JwtBearerOptions options, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var settings = IdentitySettings.Read(configuration, environment);
        options.Authority = settings.Authority;
        options.MetadataAddress = settings.MetadataAddress;
        options.Audience = settings.Audience;
        options.RequireHttpsMetadata = !environment.IsDevelopment();
        options.MapInboundClaims = false;
        options.IncludeErrorDetails = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Authority,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    }
}
