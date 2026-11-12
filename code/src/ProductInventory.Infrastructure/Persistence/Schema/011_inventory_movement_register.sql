CREATE OR ALTER PROCEDURE dbo.InventoryMovement_Register
    @Id uniqueidentifier, @ProductId uniqueidentifier, @Type int, @Quantity int, @Status int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @Quantity IS NULL OR @Quantity <= 0 OR @Type IS NULL OR @Type NOT IN (0, 1)
        THROW 51002, 'Movement requires a supported type and a positive quantity.', 1;
    DECLARE @Delta bigint = CASE WHEN @Type = 0 THEN CONVERT(bigint, @Quantity) ELSE -CONVERT(bigint, @Quantity) END;
    DECLARE @Balance TABLE (StockBefore int, StockAfter int);
    BEGIN TRY
        BEGIN TRANSACTION;
        UPDATE dbo.Products
        SET StockQuantity = CONVERT(int, CONVERT(bigint, StockQuantity) + @Delta)
        OUTPUT deleted.StockQuantity, inserted.StockQuantity INTO @Balance
        WHERE Id = @ProductId AND CONVERT(bigint, StockQuantity) + @Delta BETWEEN 0 AND 2147483647;
        IF NOT EXISTS (SELECT 1 FROM @Balance)
        BEGIN
            SET @Status = CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Id = @ProductId) THEN 1
                WHEN @Type = 1 THEN 2 ELSE 3 END;
            COMMIT TRANSACTION;

            RETURN;
        END;
        DECLARE @OccurredAt datetimeoffset(7) = TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00');
        INSERT dbo.InventoryMovements(Id, ProductId, Type, Quantity, OccurredAt, StockBefore, StockAfter)
        SELECT @Id, @ProductId, CASE WHEN @Type = 0 THEN 'IN' ELSE 'OUT' END, @Quantity, @OccurredAt, StockBefore, StockAfter FROM @Balance;
        SET @Status = 0;
        COMMIT TRANSACTION;
        SELECT @Id AS Id, @ProductId AS ProductId, @Type AS Type, @Quantity AS Quantity,
            @OccurredAt AS OccurredAt, StockBefore, StockAfter FROM @Balance;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
