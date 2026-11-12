CREATE OR ALTER PROCEDURE dbo.Supplier_Create
    @Id uniqueidentifier, @Name nvarchar(150), @ContactEmail nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Suppliers(Id, Name, ContactEmail)
    OUTPUT inserted.Id, inserted.Name, inserted.ContactEmail
    VALUES(@Id, @Name, @ContactEmail);
END;
