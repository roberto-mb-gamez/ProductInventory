SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @lockResult int;
EXEC @lockResult = sys.sp_getapplock @Resource = 'ProductInventory.Schema', @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 60000;
IF @lockResult < 0 THROW 51000, 'Unable to acquire schema initialization lock.', 1;
IF OBJECT_ID('dbo.Categories', 'U') IS NULL
CREATE TABLE dbo.Categories (
    Id uniqueidentifier NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    Name nvarchar(100) COLLATE Latin1_General_100_CI_AS NOT NULL,
    Description nvarchar(max) NULL,
    CONSTRAINT UQ_Categories_Name UNIQUE (Name),
    CONSTRAINT CK_Categories_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
IF OBJECT_ID('dbo.Suppliers', 'U') IS NULL
CREATE TABLE dbo.Suppliers (
    Id uniqueidentifier NOT NULL CONSTRAINT PK_Suppliers PRIMARY KEY,
    Name nvarchar(150) NOT NULL,
    ContactEmail nvarchar(max) NULL,
    CONSTRAINT CK_Suppliers_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
IF OBJECT_ID('dbo.Products', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
        Sku nvarchar(40) COLLATE Latin1_General_100_CI_AS NOT NULL,
        Name nvarchar(120) NOT NULL,
        Description nvarchar(max) NULL,
        Price decimal(19,4) NOT NULL,
        StockQuantity int NOT NULL CONSTRAINT DF_Products_StockQuantity DEFAULT (0),
        CategoryId uniqueidentifier NOT NULL,
        SupplierId uniqueidentifier NOT NULL,
        CONSTRAINT UQ_Products_Sku UNIQUE (Sku),
        CONSTRAINT CK_Products_Sku CHECK (LEN(LTRIM(RTRIM(Sku))) > 0),
        CONSTRAINT CK_Products_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
        CONSTRAINT CK_Products_Price CHECK (Price >= 0),
        CONSTRAINT CK_Products_Stock CHECK (StockQuantity >= 0),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
        CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
    );
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
    CREATE INDEX IX_Products_SupplierId ON dbo.Products(SupplierId);
END;
IF OBJECT_ID('dbo.InventoryMovements', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryMovements (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_InventoryMovements PRIMARY KEY,
        ProductId uniqueidentifier NOT NULL,
        Type varchar(3) COLLATE Latin1_General_100_BIN2 NOT NULL,
        Quantity int NOT NULL,
        OccurredAt datetimeoffset(7) NOT NULL,
        StockBefore int NOT NULL,
        StockAfter int NOT NULL,
        CONSTRAINT FK_InventoryMovements_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
        CONSTRAINT CK_InventoryMovements_Type CHECK (Type IN ('IN', 'OUT')),
        CONSTRAINT CK_InventoryMovements_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_InventoryMovements_Stock CHECK (StockBefore >= 0 AND StockAfter >= 0),
        CONSTRAINT CK_InventoryMovements_Utc CHECK (DATEPART(TZOFFSET, OccurredAt) = 0),
        CONSTRAINT CK_InventoryMovements_Balance CHECK (
            (Type = 'IN' AND CONVERT(bigint, StockAfter) = CONVERT(bigint, StockBefore) + Quantity) OR
            (Type = 'OUT' AND CONVERT(bigint, StockAfter) = CONVERT(bigint, StockBefore) - Quantity))
    );
    CREATE INDEX IX_InventoryMovements_ProductId_OccurredAt ON dbo.InventoryMovements(ProductId, OccurredAt, Id);
END;
EXEC(N'CREATE OR ALTER TRIGGER dbo.TR_InventoryMovements_Immutable ON dbo.InventoryMovements AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    THROW 51001, ''Inventory movements cannot be changed or deleted.'', 1;
END;');
COMMIT TRANSACTION;
