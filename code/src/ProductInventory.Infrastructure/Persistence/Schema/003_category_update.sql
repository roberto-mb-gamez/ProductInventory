CREATE OR ALTER PROCEDURE dbo.Category_Update
    @Id uniqueidentifier, @Name nvarchar(100), @Description nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Categories SET Name = @Name, Description = @Description
    OUTPUT inserted.Id, inserted.Name, inserted.Description
    WHERE Id = @Id;
END;
