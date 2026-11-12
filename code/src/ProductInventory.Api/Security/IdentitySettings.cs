namespace ProductInventory.Api.Security;
public sealed record IdentitySettings(string Authority, string MetadataAddress, string Audience)
{
    public static IdentitySettings Read(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var authority = GetRequired(configuration, "Identity:Authority").TrimEnd('/');
        var metadata = configuration["Identity:MetadataAddress"] ?? $"{authority}/.well-known/openid-configuration";
        ValidateAddress(authority, environment);
        ValidateAddress(metadata, environment);

        return new(authority, metadata, GetRequired(configuration, "Identity:Audience"));
    }
    internal static string GetRequired(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"The environment setting {key.Replace(":", "__")} is required.");

        return value;
    }
    private static void ValidateAddress(string value, IWebHostEnvironment environment)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var address) || address.Scheme is not ("http" or "https")
            || address.UserInfo.Length != 0 || address.Fragment.Length != 0 || address.Query.Length != 0)
            throw new InvalidOperationException("Identity endpoints must be absolute HTTP(S) URLs without credentials, queries or fragments.");
        if (!environment.IsDevelopment() && address.Scheme != "https")
            throw new InvalidOperationException("Identity endpoints require HTTPS outside Development.");
    }
}
