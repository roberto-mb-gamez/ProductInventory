CREATE OR ALTER PROCEDURE dbo.Supplier_Delete
    @Id uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    DELETE dbo.Suppliers WHERE Id = @Id;
    SELECT CONVERT(bit, @@ROWCOUNT);
END;
