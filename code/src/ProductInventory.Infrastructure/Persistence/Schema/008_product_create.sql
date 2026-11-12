CREATE OR ALTER PROCEDURE dbo.Product_Create
    @Id uniqueidentifier, @Sku nvarchar(40), @Name nvarchar(120),
    @Description nvarchar(max), @Price decimal(38,4),
    @CategoryId uniqueidentifier, @SupplierId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Products(Id, Sku, Name, Description, Price, CategoryId, SupplierId)
    OUTPUT inserted.Id, inserted.Sku, inserted.Name, inserted.Description, inserted.Price,
        inserted.StockQuantity, inserted.CategoryId, inserted.SupplierId
    VALUES(@Id, @Sku, @Name, @Description, @Price, @CategoryId, @SupplierId);
END;
