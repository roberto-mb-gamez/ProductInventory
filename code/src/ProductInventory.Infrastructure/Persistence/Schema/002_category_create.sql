CREATE OR ALTER PROCEDURE dbo.Category_Create
    @Id uniqueidentifier, @Name nvarchar(100), @Description nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Categories(Id, Name, Description)
    OUTPUT inserted.Id, inserted.Name, inserted.Description
    VALUES(@Id, @Name, @Description);
END;
