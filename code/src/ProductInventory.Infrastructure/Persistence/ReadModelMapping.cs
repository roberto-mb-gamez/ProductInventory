using Microsoft.EntityFrameworkCore;
using ProductInventory.Infrastructure.Persistence.ReadModels;
namespace ProductInventory.Infrastructure.Persistence;
internal static class ReadModelMapping
{
    public static void MapCatalogs(ModelBuilder modelBuilder)
    {
        var category = modelBuilder.Entity<CategoryRow>();
        category.ToTable("Categories", "dbo");
        category.HasKey(row => row.Id);
        category.Property(row => row.Name).HasMaxLength(100).UseCollation("Latin1_General_100_CI_AS");
        category.HasIndex(row => row.Name).IsUnique();
        var supplier = modelBuilder.Entity<SupplierRow>();
        supplier.ToTable("Suppliers", "dbo");
        supplier.HasKey(row => row.Id);
        supplier.Property(row => row.Name).HasMaxLength(150);
    }
    public static void MapProducts(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<ProductRow>();
        product.ToTable("Products", "dbo");
        product.HasKey(row => row.Id);
        product.Property(row => row.Sku).HasMaxLength(40).UseCollation("Latin1_General_100_CI_AS");
        product.HasIndex(row => row.Sku).IsUnique();
        product.Property(row => row.Name).HasMaxLength(120);
        product.Property(row => row.Price).HasPrecision(19, 4);
        product.Property(row => row.StockQuantity).HasDefaultValue(0);
        product.HasOne(row => row.Category).WithMany().HasForeignKey(row => row.CategoryId).OnDelete(DeleteBehavior.NoAction);
        product.HasOne(row => row.Supplier).WithMany().HasForeignKey(row => row.SupplierId).OnDelete(DeleteBehavior.NoAction);
    }
    public static void MapMovements(ModelBuilder modelBuilder)
    {
        var movement = modelBuilder.Entity<StockMovementRow>();
        movement.ToTable("InventoryMovements", "dbo");
        movement.HasKey(row => row.Id);
        movement.Property(row => row.Type).HasMaxLength(3).IsUnicode(false).UseCollation("Latin1_General_100_BIN2");
        movement.HasIndex(row => new { row.ProductId, row.OccurredAt, row.Id });
        movement.HasOne<ProductRow>().WithMany().HasForeignKey(row => row.ProductId).OnDelete(DeleteBehavior.NoAction);
    }
}
