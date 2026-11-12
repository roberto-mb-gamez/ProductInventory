using ProductInventory.Application.Pagination;
namespace ProductInventory.Application.Products;
public sealed record ListProductsQuery(PageRequest Page, ProductFilter Filter);
