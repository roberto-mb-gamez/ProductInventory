CREATE OR ALTER PROCEDURE dbo.Product_Delete
    @Id uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    DELETE dbo.Products WHERE Id = @Id;
    SELECT CONVERT(bit, @@ROWCOUNT);
END;
