namespace ProductInventory.IntegrationTests.Persistence;
public sealed record MovementConstraintCase(string Type, int Quantity, int StockBefore, int StockAfter, int OffsetMinutes);
