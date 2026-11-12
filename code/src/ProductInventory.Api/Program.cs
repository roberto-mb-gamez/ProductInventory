namespace ProductInventory.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddHealthChecks();
        var app = builder.Build();
        app.MapHealthChecks("/health");
        app.Run();
    }
}
