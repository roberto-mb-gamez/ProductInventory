namespace ProductInventory.Application.Suppliers;
public sealed record UpdateSupplierCommand(Guid Id, SupplierInput Data);
