namespace ProductInventory.Api.Security;
public static class ApiTransportSecurity
{
    public static void UseApiTransportSecurity(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            app.Use(RequireHttpsAsync);
    }
    private static Task RequireHttpsAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.IsHttps)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            return Task.CompletedTask;
        }

        return next(context);
    }
}
