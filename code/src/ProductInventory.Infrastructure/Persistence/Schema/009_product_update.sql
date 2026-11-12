CREATE OR ALTER PROCEDURE dbo.Product_Update
    @Id uniqueidentifier, @Sku nvarchar(40), @Name nvarchar(120),
    @Description nvarchar(max), @Price decimal(38,4),
    @CategoryId uniqueidentifier, @SupplierId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Products SET Sku = @Sku, Name = @Name, Description = @Description,
        Price = @Price, CategoryId = @CategoryId, SupplierId = @SupplierId
    OUTPUT inserted.Id, inserted.Sku, inserted.Name, inserted.Description, inserted.Price,
        inserted.StockQuantity, inserted.CategoryId, inserted.SupplierId
    WHERE Id = @Id;
END;
