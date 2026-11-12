namespace ProductInventory.Infrastructure.Persistence;
public sealed record SqlServerSettings(string ConnectionString, bool InitializeDatabase = false);
