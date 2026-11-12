using Microsoft.EntityFrameworkCore;
using ProductInventory.Infrastructure.Persistence.ReadModels;
namespace ProductInventory.Infrastructure.Persistence;
public sealed class InventoryReadDbContext(DbContextOptions<InventoryReadDbContext> options) : DbContext(options)
{
    public DbSet<ProductRow> Products => Set<ProductRow>();
    public DbSet<CategoryRow> Categories => Set<CategoryRow>();
    public DbSet<SupplierRow> Suppliers => Set<SupplierRow>();
    public DbSet<StockMovementRow> InventoryMovements => Set<StockMovementRow>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ReadModelMapping.MapCatalogs(modelBuilder);
        ReadModelMapping.MapProducts(modelBuilder);
        ReadModelMapping.MapMovements(modelBuilder);
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotSupportedException("The EF Core context is read-only. Use Dapper write ports.");
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("The EF Core context is read-only. Use Dapper write ports.");
    }
}
