﻿// <auto-generated />
using System;
using Infrastructure.Identity.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InventoryManagementSystem.PublicApi.Migrations
{
    [DbContext(typeof(IdentityAndProductDbContext))]
    partial class IdentityAndProductDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("DateOfBirthNormalizedString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Fullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("OwnerID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("SystemUser");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Notification", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Channel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsIssueOrder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CustomerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeliveryMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GoodsIssueType")
                        .HasColumnType("int");

                    b.Property<string>("RequestId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupplierId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.HasIndex("TransactionId");

                    b.ToTable("GoodsIssueOrder");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LocationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PurchaseOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SupplierId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SupplierInvoice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("PurchaseOrderId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("TransactionId");

                    b.ToTable("GoodsReceiptOrder");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrderItem", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GoodsReceiptOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProductVariantId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProductVariantName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuantityReceived")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GoodsReceiptOrderId");

                    b.HasIndex("ProductVariantId");

                    b.ToTable("GoodsReceiptOrderItems");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.OrderItem", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("DiscountAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("GoodsIssueOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderQuantity")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductVariantId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PurchaseOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GoodsIssueOrderId");

                    b.HasIndex("ProductVariantId");

                    b.HasIndex("PurchaseOrderId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.PurchaseOrder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeliveryAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("MailDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PurchaseOrderStatus")
                        .HasColumnType("int");

                    b.Property<string>("SupplierId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("TotalDiscountAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalOrderAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("WarehouseLocation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.HasIndex("TransactionId");

                    b.ToTable("PurchaseOrder");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.StockTakeItem", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ActualQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductVariantId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("StockTakeOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ProductVariantId");

                    b.HasIndex("StockTakeOrderId");

                    b.ToTable("StockTakeItem");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.StockTakeOrder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("StockTakeOrderType")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("StockTakeOrder");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.Supplier", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SalePersonName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("Supplier");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("TransactionStatus")
                        .HasColumnType("bit");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.TransactionRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("UserTransactionActionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionRecord");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Brand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BrandDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BrandName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoryDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Location", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LocationName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Package", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GoodsReceiptOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ImportedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LocationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductVariantId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SupplierId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GoodsReceiptOrderId");

                    b.HasIndex("LocationId");

                    b.HasIndex("ProductVariantId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("TransactionId");

                    b.ToTable("Package");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BrandId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoryId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsVariantType")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SellingStrategy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("TransactionId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Barcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsVariantType")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Sku")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StorageQuantity")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("TransactionId");

                    b.ToTable("ProductVariant");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaim");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaim");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsIssueOrder", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Supplier");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrder", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.PurchaseOrder", "PurchaseOrder")
                        .WithMany()
                        .HasForeignKey("PurchaseOrderId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Location");

                    b.Navigation("PurchaseOrder");

                    b.Navigation("Supplier");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrderItem", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrder", "GoodsReceiptOrder")
                        .WithMany("ReceivedOrderItems")
                        .HasForeignKey("GoodsReceiptOrderId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", "ProductVariant")
                        .WithMany()
                        .HasForeignKey("ProductVariantId");

                    b.Navigation("GoodsReceiptOrder");

                    b.Navigation("ProductVariant");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.OrderItem", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsIssueOrder", null)
                        .WithMany("GoodsIssueProducts")
                        .HasForeignKey("GoodsIssueOrderId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", "ProductVariant")
                        .WithMany()
                        .HasForeignKey("ProductVariantId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.PurchaseOrder", null)
                        .WithMany("PurchaseOrderProduct")
                        .HasForeignKey("PurchaseOrderId");

                    b.Navigation("ProductVariant");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.PurchaseOrder", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Supplier");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.StockTakeItem", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", "ProductVariant")
                        .WithMany()
                        .HasForeignKey("ProductVariantId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.StockTakeOrder", "StockTakeOrder")
                        .WithMany("CheckItems")
                        .HasForeignKey("StockTakeOrderId");

                    b.Navigation("ProductVariant");

                    b.Navigation("StockTakeOrder");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.StockTakeOrder", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.Supplier", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.TransactionRecord", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany("TransactionRecord")
                        .HasForeignKey("TransactionId");

                    b.Navigation("ApplicationUser");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Category", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Package", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrder", "GoodsReceiptOrder")
                        .WithMany()
                        .HasForeignKey("GoodsReceiptOrderId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", "ProductVariant")
                        .WithMany("Packages")
                        .HasForeignKey("ProductVariantId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("GoodsReceiptOrder");

                    b.Navigation("Location");

                    b.Navigation("ProductVariant");

                    b.Navigation("Supplier");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Product", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.Brand", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Brand");

                    b.Navigation("Category");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Products.Product", "Product")
                        .WithMany("ProductVariants")
                        .HasForeignKey("ProductId");

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId");

                    b.Navigation("Product");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("InventoryManagementSystem.ApplicationCore.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsIssueOrder", b =>
                {
                    b.Navigation("GoodsIssueProducts");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.GoodsReceiptOrder", b =>
                {
                    b.Navigation("ReceivedOrderItems");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.PurchaseOrder", b =>
                {
                    b.Navigation("PurchaseOrderProduct");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.StockTakeOrder", b =>
                {
                    b.Navigation("CheckItems");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Orders.Transaction", b =>
                {
                    b.Navigation("TransactionRecord");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Brand", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.Product", b =>
                {
                    b.Navigation("ProductVariants");
                });

            modelBuilder.Entity("InventoryManagementSystem.ApplicationCore.Entities.Products.ProductVariant", b =>
                {
                    b.Navigation("Packages");
                });
#pragma warning restore 612, 618
        }
    }
}
