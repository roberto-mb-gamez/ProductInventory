using ProductInventory.Application.Categories;
using ProductInventory.Application.Suppliers;
using ProductInventory.Application.Products;
using ProductInventory.Application.Inventory;
namespace ProductInventory.Api.Hosting;
public static class ApplicationRegistration
{
    public static void AddApplicationHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();
        services.AddScoped<GetCategoryHandler>();
        services.AddScoped<ListCategoriesHandler>();
        services.AddScoped<CreateSupplierHandler>();
        services.AddScoped<UpdateSupplierHandler>();
        services.AddScoped<DeleteSupplierHandler>();
        services.AddScoped<GetSupplierHandler>();
        services.AddScoped<ListSuppliersHandler>();
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<DeleteProductHandler>();
        services.AddScoped<GetProductHandler>();
        services.AddScoped<ListProductsHandler>();
        services.AddScoped<RegisterStockMovementHandler>();
    }
}
