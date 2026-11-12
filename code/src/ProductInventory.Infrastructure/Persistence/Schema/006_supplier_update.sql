CREATE OR ALTER PROCEDURE dbo.Supplier_Update
    @Id uniqueidentifier, @Name nvarchar(150), @ContactEmail nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Suppliers SET Name = @Name, ContactEmail = @ContactEmail
    OUTPUT inserted.Id, inserted.Name, inserted.ContactEmail
    WHERE Id = @Id;
END;
