CREATE OR ALTER PROCEDURE dbo.Category_Delete
    @Id uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    DELETE dbo.Categories WHERE Id = @Id;
    SELECT CONVERT(bit, @@ROWCOUNT);
END;
