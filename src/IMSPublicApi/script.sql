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

CREATE TABLE [Category] (
    [Id] nvarchar(450) NOT NULL,
    [CategoryName] nvarchar(max) NULL,
    [CategoryDescription] nvarchar(max) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([Id])
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

CREATE TABLE [Supplier] (
    [Id] nvarchar(450) NOT NULL,
    [SupplierName] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Street] nvarchar(50) NOT NULL,
    [City] nvarchar(30) NOT NULL,
    [Province] nvarchar(30) NOT NULL,
    [Country] nvarchar(30) NOT NULL,
    [SalePersonName] nvarchar(50) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [UserInfo] (
    [Id] nvarchar(450) NOT NULL,
    [OwnerID] nvarchar(max) NULL,
    [Username] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Fullname] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [DateOfBirthNormalizedString] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_UserInfo] PRIMARY KEY ([Id])
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

CREATE TABLE [Product] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NULL,
    [BrandId] nvarchar(450) NULL,
    [CategoryId] nvarchar(450) NULL,
    [CreatedById] nvarchar(450) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [SellingStrategy] nvarchar(max) NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Product_Brand_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [Brand] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Product_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Product_UserInfo_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [UserInfo] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [SystemUser] (
    [Id] nvarchar(450) NOT NULL,
    [UserInfoId] nvarchar(450) NULL,
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
    CONSTRAINT [PK_SystemUser] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SystemUser_UserInfo_UserInfoId] FOREIGN KEY ([UserInfoId]) REFERENCES [UserInfo] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Transaction] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NULL,
    [ValidUntil] datetime2 NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [CreatedById] nvarchar(450) NULL,
    [ConfirmedById] nvarchar(450) NULL,
    [ModifiedById] nvarchar(450) NULL,
    [Type] int NOT NULL,
    [TransactionStatus] bit NOT NULL,
    CONSTRAINT [PK_Transaction] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transaction_UserInfo_ConfirmedById] FOREIGN KEY ([ConfirmedById]) REFERENCES [UserInfo] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Transaction_UserInfo_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [UserInfo] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Transaction_UserInfo_ModifiedById] FOREIGN KEY ([ModifiedById]) REFERENCES [UserInfo] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ProductVariant] (
    [Id] nvarchar(450) NOT NULL,
    [ProductId] nvarchar(450) NULL,
    [Name] nvarchar(max) NULL,
    [Price] decimal(18,2) NOT NULL,
    [Sku] nvarchar(max) NULL,
    [Unit] nvarchar(max) NULL,
    [StorageQuantity] int NOT NULL,
    [StorageLocation] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NOT NULL,
    [IsVariantType] bit NOT NULL,
    CONSTRAINT [PK_ProductVariant] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariant_Product_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE NO ACTION
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

CREATE TABLE [GoodsIssueOrders] (
    [Id] nvarchar(450) NOT NULL,
    [GoodsIssueNumber] nvarchar(max) NULL,
    [RequestId] nvarchar(max) NULL,
    [DeliveryMethod] nvarchar(max) NULL,
    [CustomerName] nvarchar(max) NULL,
    [SupplierId] nvarchar(450) NULL,
    [TransactionId] nvarchar(450) NULL,
    [GoodsIssueType] int NOT NULL,
    [DeliveryDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GoodsIssueOrders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsIssueOrders_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsIssueOrders_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PriceQuote] (
    [Id] nvarchar(450) NOT NULL,
    [PriceQuoteNumber] nvarchar(max) NULL,
    [SupplierId] nvarchar(450) NULL,
    [Deadline] datetime2 NOT NULL,
    [MailDescription] nvarchar(max) NULL,
    [PriceQuoteStatus] int NOT NULL,
    [TotalOrderAmount] decimal(18,2) NOT NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_PriceQuote] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PriceQuote_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PriceQuote_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ProductSerialNumber] (
    [Id] nvarchar(450) NOT NULL,
    [ProductId] nvarchar(max) NULL,
    [SerialNumber] nvarchar(max) NULL,
    [ProductVariantId] nvarchar(450) NULL,
    CONSTRAINT [PK_ProductSerialNumber] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductSerialNumber_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VariantValue] (
    [Id] nvarchar(450) NOT NULL,
    [ProductVariantId] nvarchar(450) NULL,
    [Attribute] nvarchar(max) NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_VariantValue] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VariantValue_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PurchaseOrder] (
    [Id] nvarchar(450) NOT NULL,
    [PurchaseOrderNumber] nvarchar(max) NULL,
    [DeliveryDate] datetime2 NOT NULL,
    [DeliveryAddress] nvarchar(max) NULL,
    [MailDescription] nvarchar(max) NULL,
    [SupplierId] nvarchar(450) NULL,
    [WarehouseLocation] nvarchar(max) NULL,
    [PurchaseOrderStatus] int NOT NULL,
    [TotalDiscountAmount] decimal(18,2) NOT NULL,
    [TotalOrderAmount] decimal(18,2) NOT NULL,
    [PriceQuoteOrderId] nvarchar(450) NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseOrder_PriceQuote_PriceQuoteOrderId] FOREIGN KEY ([PriceQuoteOrderId]) REFERENCES [PriceQuote] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrder_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrder_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GoodsReceiptOrder] (
    [Id] nvarchar(450) NOT NULL,
    [GoodsReceiptOrderNumber] nvarchar(max) NULL,
    [PurchaseOrderId] nvarchar(450) NULL,
    [ReceivedDate] datetime2 NOT NULL,
    [SupplierId] nvarchar(450) NULL,
    [SupplierInvoice] nvarchar(max) NULL,
    [WarehouseLocation] nvarchar(max) NULL,
    [TransactionId] nvarchar(450) NULL,
    CONSTRAINT [PK_GoodsReceiptOrder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsReceiptOrder_PurchaseOrder_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrder] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrder_Supplier_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Supplier] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrder_Transaction_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transaction] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [OrderItem] (
    [Id] nvarchar(450) NOT NULL,
    [OrderNumber] nvarchar(max) NULL,
    [ProductVariantId] nvarchar(450) NULL,
    [OrderQuantity] int NOT NULL,
    [Unit] nvarchar(max) NULL,
    [Price] decimal(18,2) NOT NULL,
    [DiscountAmount] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [GoodsIssueOrderId] nvarchar(450) NULL,
    [PriceQuoteOrderId] nvarchar(450) NULL,
    [PurchaseOrderId] nvarchar(450) NULL,
    CONSTRAINT [PK_OrderItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItem_GoodsIssueOrders_GoodsIssueOrderId] FOREIGN KEY ([GoodsIssueOrderId]) REFERENCES [GoodsIssueOrders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderItem_PriceQuote_PriceQuoteOrderId] FOREIGN KEY ([PriceQuoteOrderId]) REFERENCES [PriceQuote] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderItem_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderItem_PurchaseOrder_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrder] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GoodsReceiptOrderItems] (
    [Id] nvarchar(450) NOT NULL,
    [ReceivedOrderId] nvarchar(max) NULL,
    [ReceivingOrderId] nvarchar(450) NULL,
    [StorageLocation] nvarchar(max) NULL,
    [ProductVariantId] nvarchar(450) NULL,
    [Quantity] int NOT NULL,
    [QuantityReceived] int NOT NULL,
    [QuantityInventory] int NOT NULL,
    CONSTRAINT [PK_GoodsReceiptOrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsReceiptOrderItems_GoodsReceiptOrder_ReceivingOrderId] FOREIGN KEY ([ReceivingOrderId]) REFERENCES [GoodsReceiptOrder] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptOrderItems_ProductVariant_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariant] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_GoodsIssueOrders_SupplierId] ON [GoodsIssueOrders] ([SupplierId]);
GO

CREATE INDEX [IX_GoodsIssueOrders_TransactionId] ON [GoodsIssueOrders] ([TransactionId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_PurchaseOrderId] ON [GoodsReceiptOrder] ([PurchaseOrderId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_SupplierId] ON [GoodsReceiptOrder] ([SupplierId]);
GO

CREATE INDEX [IX_GoodsReceiptOrder_TransactionId] ON [GoodsReceiptOrder] ([TransactionId]);
GO

CREATE INDEX [IX_GoodsReceiptOrderItems_ProductVariantId] ON [GoodsReceiptOrderItems] ([ProductVariantId]);
GO

CREATE INDEX [IX_GoodsReceiptOrderItems_ReceivingOrderId] ON [GoodsReceiptOrderItems] ([ReceivingOrderId]);
GO

CREATE INDEX [IX_OrderItem_GoodsIssueOrderId] ON [OrderItem] ([GoodsIssueOrderId]);
GO

CREATE INDEX [IX_OrderItem_PriceQuoteOrderId] ON [OrderItem] ([PriceQuoteOrderId]);
GO

CREATE INDEX [IX_OrderItem_ProductVariantId] ON [OrderItem] ([ProductVariantId]);
GO

CREATE INDEX [IX_OrderItem_PurchaseOrderId] ON [OrderItem] ([PurchaseOrderId]);
GO

CREATE INDEX [IX_PriceQuote_SupplierId] ON [PriceQuote] ([SupplierId]);
GO

CREATE INDEX [IX_PriceQuote_TransactionId] ON [PriceQuote] ([TransactionId]);
GO

CREATE INDEX [IX_Product_BrandId] ON [Product] ([BrandId]);
GO

CREATE INDEX [IX_Product_CategoryId] ON [Product] ([CategoryId]);
GO

CREATE INDEX [IX_Product_CreatedById] ON [Product] ([CreatedById]);
GO

CREATE INDEX [IX_ProductSerialNumber_ProductVariantId] ON [ProductSerialNumber] ([ProductVariantId]);
GO

CREATE INDEX [IX_ProductVariant_ProductId] ON [ProductVariant] ([ProductId]);
GO

CREATE INDEX [IX_PurchaseOrder_PriceQuoteOrderId] ON [PurchaseOrder] ([PriceQuoteOrderId]);
GO

CREATE INDEX [IX_PurchaseOrder_SupplierId] ON [PurchaseOrder] ([SupplierId]);
GO

CREATE INDEX [IX_PurchaseOrder_TransactionId] ON [PurchaseOrder] ([TransactionId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [Role] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_RoleClaim_RoleId] ON [RoleClaim] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [SystemUser] ([NormalizedEmail]);
GO

CREATE INDEX [IX_SystemUser_UserInfoId] ON [SystemUser] ([UserInfoId]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [SystemUser] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Transaction_ConfirmedById] ON [Transaction] ([ConfirmedById]);
GO

CREATE INDEX [IX_Transaction_CreatedById] ON [Transaction] ([CreatedById]);
GO

CREATE INDEX [IX_Transaction_ModifiedById] ON [Transaction] ([ModifiedById]);
GO

CREATE INDEX [IX_UserClaim_UserId] ON [UserClaim] ([UserId]);
GO

CREATE INDEX [IX_UserLogin_UserId] ON [UserLogin] ([UserId]);
GO

CREATE INDEX [IX_UserRole_RoleId] ON [UserRole] ([RoleId]);
GO

CREATE INDEX [IX_VariantValue_ProductVariantId] ON [VariantValue] ([ProductVariantId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210609020711_Creation', N'5.0.5');
GO

COMMIT;
GO

