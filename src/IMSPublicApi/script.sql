IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    )
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Brand] (
    [Id] nvarchar(450) NOT NULL,
    [BrandName] nvarchar(max) NULL,
    [BrandDescription] nvarchar(max) NULL,
    CONSTRAINT [PK_Brand] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Notification] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(max) NULL,
    [UserName] nvarchar(max) NULL,
    [Channel] nvarchar(max) NULL,
    [Message] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Role] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SystemUser] (
    [Id] nvarchar(450) NOT NULL,
    [OwnerID] nvarchar(max) NULL,
    [Fullname] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [DateOfBirthNormalizedString] nvarchar(max) NULL,
    [ProfileImageLink] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_SystemUser] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Transaction] (
    [Id] nvarchar(450) NOT NULL,
    [Type] int NOT NULL,
    [TransactionStatus] bit NOT NULL,
    CONSTRAINT [PK_Transaction] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [RoleClaim] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_RoleClaim] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaim_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserClaim] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_UserClaim] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaim_SystemUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserLogin] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserLogin] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_UserLogin_SystemUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserRole] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_SystemUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserToken] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_UserToken] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_UserToken_SystemUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [SystemUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Category] (
    [Id] nvarchar(450) NOT NULL,
    [CategoryName] nvarchar(max) NULL,
    [CategoryDescription] nvarchar(max) NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Category_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Location] (
    [Id] nvarchar(450) NOT NULL,
    [LocationBarcode] nvarchar(max) NULL,
    [LocationName] nvarchar(max) NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_Location] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Location_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StockTakeOrder] (
    [Id] nvarchar(450) NOT NULL,
    [TransactionId] nvarchar(450) NULL,
    [StockTakeOrderType] int NOT NULL,
    CONSTRAINT [PK_StockTakeOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockTakeOrder_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Supplier] (
    [Id] nvarchar(450) NOT NULL,
    [SupplierName] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [SalePersonName] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Supplier_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TransactionRecord] (
    [Id] nvarchar(450) NOT NULL,
    [Date] datetime2 NOT NULL,
    [TransactionId] nvarchar(450) NULL,
    [OrderId] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [ApplicationUserId] nvarchar(450) NULL,
    [UserTransactionActionType] int NOT NULL,
    CONSTRAINT [PK_TransactionRecord] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TransactionRecord_SystemUser_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [SystemUser] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TransactionRecord_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Product] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NULL,
    [BrandId] nvarchar(450) NULL,
    [Unit] nvarchar(max) NULL,
    [CategoryId] nvarchar(450) NULL,
    [TransactionId] nvarchar(450) NULL,
    [SellingStrategy] nvarchar(max) NULL,
    [ProductImageLink] nvarchar(max) NULL,
    [IsVariantType] bit NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Product_Brand_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [Brand] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Product_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Product_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StockTakeGroupLocation] (
    [Id] nvarchar(450) NOT NULL,
    [LocationId] nvarchar(450) NULL,
    [StockTakeOrderId] nvarchar(450) NULL,
    CONSTRAINT [PK_StockTakeGroupLocation] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockTakeGroupLocation_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Location] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StockTakeGroupLocation_StockTakeOrder_StockTakeOrderId] FOREIGN KEY ([StockTakeOrderId]) REFERENCES [StockTakeOrder] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GoodsIssueOrder] (
    [Id] nvarchar(450) NOT NULL,
    [RequestId] nvarchar(max) NULL,
    [DeliveryMethod] nvarchar(max) NULL,
    [DeliveryAddress] nvarchar(max) NULL,
    [CustomerName] nvarchar(max) NULL,
    [CustomerPhoneNumber] nvarchar(max) NULL,
    [SupplierId] nvarchar(450) NULL,
    [TransactionId] nvarchar(450) NULL,
    [GoodsIssueType] int NOT NULL,
    [DeliveryDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GoodsIssueOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsIssueOrder_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsIssueOrder_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PurchaseOrder] (
    [Id] nvarchar(450) NOT NULL,
    [DeliveryDate] datetime2 NOT NULL,
    [DeliveryAddress] nvarchar(max) NULL,
    [MergedWithPurchaseOrderId] nvarchar(max) NULL,
    [MailDescription] nvarchar(max) NULL,
    [SupplierId] nvarchar(450) NULL,
    [WarehouseLocation] nvarchar(max) NULL,
    [PurchaseOrderStatus] int NOT NULL,
    [TotalDiscountAmount] decimal(18,2) NOT NULL,
    [TotalOrderAmount] decimal(18,2) NOT NULL,
    [TransactionId] nvarchar(450) NULL,
    [Deadline] datetime2 NOT NULL,
    [HasBeenModified] bit NOT NULL,
    CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseOrder_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrder_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ProductVariant] (
    [Id] nvarchar(450) NOT NULL,
    [ProductId] nvarchar(450) NULL,
    [Name] nvarchar(max) NULL,
    [Barcode] nvarchar(max) NULL,
    [Price] decimal(18,2) NOT NULL,
    [Cost] decimal(18,2) NOT NULL,
    [Sku] nvarchar(max) NULL,
    [StorageQuantity] int NOT NULL,
    [TransactionId] nvarchar(450) NULL,
    [VariantImageLink] nvarchar(max) NULL,
    [IsVariantType] bit NOT NULL,
    CONSTRAINT [PK_ProductVariant] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariant_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProductVariant_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GoodsReceiptOrder] (
    [Id] nvarchar(450) NOT NULL,
    [PurchaseOrderId] nvarchar(450) NULL,
    [LocationId] nvarchar(450) NULL,
    [ReceivedDate] datetime2 NOT NULL,
    [SupplierId] nvarchar(450) NULL,
    [SupplierInvoice] nvarchar(max) NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_GoodsReceiptOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsReceiptOrder_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Location] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrder_PurchaseOrder_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrder] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrder_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrder_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [OrderItem] (
    [Id] nvarchar(450) NOT NULL,
    [OrderId] nvarchar(max) NULL,
    [ProductVariantId] nvarchar(450) NULL,
    [OrderQuantity] int NOT NULL,
    [Unit] nvarchar(max) NULL,
    [Price] decimal(18,2) NOT NULL,
    [SalePrice] decimal(18,2) NOT NULL,
    [DiscountAmount] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [GoodsIssueOrderId] nvarchar(450) NULL,
    [PurchaseOrderId] nvarchar(450) NULL,
    CONSTRAINT [PK_OrderItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItem_GoodsIssueOrder_GoodsIssueOrderId] FOREIGN KEY ([GoodsIssueOrderId]) REFERENCES [GoodsIssueOrder] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderItem_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderItem_PurchaseOrder_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrder] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GoodsReceiptOrderItems] (
    [Id] nvarchar(450) NOT NULL,
    [GoodsReceiptOrderId] nvarchar(450) NULL,
    [ProductVariantId] nvarchar(450) NULL,
    [ProductVariantName] nvarchar(max) NULL,
    [QuantityReceived] int NOT NULL,
    CONSTRAINT [PK_GoodsReceiptOrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsReceiptOrderItems_GoodsReceiptOrder_GoodsReceiptOrderId] FOREIGN KEY ([GoodsReceiptOrderId]) REFERENCES [GoodsReceiptOrder] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrderItems_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Package] (
    [Id] nvarchar(450) NOT NULL,
    [ProductVariantId] nvarchar(450) NULL,
    [SupplierId] nvarchar(450) NULL,
    [Price] decimal(18,2) NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [ImportedDate] datetime2 NOT NULL,
    [LocationId] nvarchar(450) NULL,
    [TransactionId] nvarchar(450) NULL,
    [GoodsReceiptOrderId] nvarchar(450) NULL,
    CONSTRAINT [PK_Package] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Package_GoodsReceiptOrder_GoodsReceiptOrderId] FOREIGN KEY ([GoodsReceiptOrderId]) REFERENCES [GoodsReceiptOrder] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Package_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Location] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Package_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Package_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Package_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StockTakeItem] (
    [Id] nvarchar(450) NOT NULL,
    [PackageId] nvarchar(450) NULL,
    [ActualQuantity] int NOT NULL,
    [Note] nvarchar(max) NULL,
    [StockTakeOrderId] nvarchar(450) NULL,
    [StockTakeGroupLocationId] nvarchar(450) NULL,
    CONSTRAINT [PK_StockTakeItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockTakeItem_Package_PackageId] FOREIGN KEY ([PackageId]) REFERENCES [Package] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StockTakeItem_StockTakeGroupLocation_StockTakeGroupLocationId] FOREIGN KEY ([StockTakeGroupLocationId]) REFERENCES [StockTakeGroupLocation] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StockTakeItem_StockTakeOrder_StockTakeOrderId] FOREIGN KEY ([StockTakeOrderId]) REFERENCES [StockTakeOrder] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Category_TransactionId] ON [Category] ([TransactionId]);
GO

CREATE INDEX [IX_GoodsIssueOrder_SupplierId] ON [GoodsIssueOrder] ([SupplierId]);
GO

CREATE INDEX [IX_GoodsIssueOrder_TransactionId] ON [GoodsIssueOrder] ([TransactionId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_LocationId] ON [GoodsReceiptOrder] ([LocationId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_PurchaseOrderId] ON [GoodsReceiptOrder] ([PurchaseOrderId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_SupplierId] ON [GoodsReceiptOrder] ([SupplierId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_TransactionId] ON [GoodsReceiptOrder] ([TransactionId]);
GO

CREATE INDEX [IX_GoodsReceiptOrderItems_GoodsReceiptOrderId] ON [GoodsReceiptOrderItems] ([GoodsReceiptOrderId]);
GO

CREATE INDEX [IX_GoodsReceiptOrderItems_ProductVariantId] ON [GoodsReceiptOrderItems] ([ProductVariantId]);
GO

CREATE INDEX [IX_Location_TransactionId] ON [Location] ([TransactionId]);
GO

CREATE INDEX [IX_OrderItem_GoodsIssueOrderId] ON [OrderItem] ([GoodsIssueOrderId]);
GO

CREATE INDEX [IX_OrderItem_ProductVariantId] ON [OrderItem] ([ProductVariantId]);
GO

CREATE INDEX [IX_OrderItem_PurchaseOrderId] ON [OrderItem] ([PurchaseOrderId]);
GO

CREATE INDEX [IX_Package_GoodsReceiptOrderId] ON [Package] ([GoodsReceiptOrderId]);
GO

CREATE INDEX [IX_Package_LocationId] ON [Package] ([LocationId]);
GO

CREATE INDEX [IX_Package_ProductVariantId] ON [Package] ([ProductVariantId]);
GO

CREATE INDEX [IX_Package_SupplierId] ON [Package] ([SupplierId]);
GO

CREATE INDEX [IX_Package_TransactionId] ON [Package] ([TransactionId]);
GO

CREATE INDEX [IX_Product_BrandId] ON [Product] ([BrandId]);
GO

CREATE INDEX [IX_Product_CategoryId] ON [Product] ([CategoryId]);
GO

CREATE INDEX [IX_Product_TransactionId] ON [Product] ([TransactionId]);
GO

CREATE INDEX [IX_ProductVariant_ProductId] ON [ProductVariant] ([ProductId]);
GO

CREATE INDEX [IX_ProductVariant_TransactionId] ON [ProductVariant] ([TransactionId]);
GO

CREATE INDEX [IX_PurchaseOrder_SupplierId] ON [PurchaseOrder] ([SupplierId]);
GO

CREATE INDEX [IX_PurchaseOrder_TransactionId] ON [PurchaseOrder] ([TransactionId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [Role] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_RoleClaim_RoleId] ON [RoleClaim] ([RoleId]);
GO

CREATE INDEX [IX_StockTakeGroupLocation_LocationId] ON [StockTakeGroupLocation] ([LocationId]);
GO

CREATE INDEX [IX_StockTakeGroupLocation_StockTakeOrderId] ON [StockTakeGroupLocation] ([StockTakeOrderId]);
GO

CREATE INDEX [IX_StockTakeItem_PackageId] ON [StockTakeItem] ([PackageId]);
GO

CREATE INDEX [IX_StockTakeItem_StockTakeGroupLocationId] ON [StockTakeItem] ([StockTakeGroupLocationId]);
GO

CREATE INDEX [IX_StockTakeItem_StockTakeOrderId] ON [StockTakeItem] ([StockTakeOrderId]);
GO

CREATE INDEX [IX_StockTakeOrder_TransactionId] ON [StockTakeOrder] ([TransactionId]);
GO

CREATE INDEX [IX_Supplier_TransactionId] ON [Supplier] ([TransactionId]);
GO

CREATE INDEX [EmailIndex] ON [SystemUser] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [SystemUser] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_TransactionRecord_ApplicationUserId] ON [TransactionRecord] ([ApplicationUserId]);
GO

CREATE INDEX [IX_TransactionRecord_TransactionId] ON [TransactionRecord] ([TransactionId]);
GO

CREATE INDEX [IX_UserClaim_UserId] ON [UserClaim] ([UserId]);
GO

CREATE INDEX [IX_UserLogin_UserId] ON [UserLogin] ([UserId]);
GO

CREATE INDEX [IX_UserRole_RoleId] ON [UserRole] ([RoleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210720045833_Creation', N'5.0.5');
GO

COMMIT;
GO

