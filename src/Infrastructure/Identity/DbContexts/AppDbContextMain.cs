﻿using InventoryManagementSystem.ApplicationCore;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Identity.DbContexts
{
    public class IdentityAndProductDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityAndProductDbContext(DbContextOptions<IdentityAndProductDbContext> options)
            : base(options)
        {
        }

        public IdentityAndProductDbContext()
        {
            
        }

        public DbSet<CompanyInfo> CompanyInfo { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransactionRecord> TransactionRecord { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<AdminControlOptions> AdminControlOptions { get; set; }

        // public DbSet<ProductSerialNumber> ProductSerialNumber { get; set; }
        public DbSet<ProductVariant> ProductVariant { get; set; }
        // public DbSet<VariantValue> VariantValue { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<GoodsReceiptOrder> GoodsReceiptOrder { get; set; }
        public DbSet<GoodsReceiptOrderItem> GoodsReceiptOrderItems { get; set; }
        
        public DbSet<GoodsIssueOrder> GoodsIssueOrder { get; set; }
        public DbSet<StockTakeOrder> StockTakeOrder { get; set; }
        public DbSet<StockTakeItem> StockTakeItem { get; set; }
        public DbSet<Package> Package { get; set; }
        public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("SystemUser");
            builder.Entity<IdentityRole>().ToTable("Role");
            // builder.Entity<ApplicationRole>().ToTable("Role");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            
            // builder.Entity<GoodsReceiptOrderItem>().HasOne(e => e.GoodsReceiptOrder).WithMany(e=>e.ReceivedOrderItems).OnDelete(DeleteBehavior.Restrict);
            // builder.Entity<PurchaseOrder>().HasMany<OrderItem>().WithOne().OnDelete(DeleteBehavior.Restrict);
            //
            // builder.Entity<Transaction>().
            //     HasOne(e => e.PurchaseOrder).WithOne(e => e.Transaction).OnDelete(DeleteBehavior.ClientCascade);
            // builder.Entity<Transaction>().
            //     HasOne(e => e.GoodsReceiptOrder).WithOne(e => e.Transaction).OnDelete(DeleteBehavior.ClientCascade);
            // builder.Entity<Transaction>().
            //     HasOne(e => e.GoodsIssueOrder).WithOne(e => e.Transaction).OnDelete(DeleteBehavior.ClientCascade);
            // builder.Entity<Transaction>().
            //     HasOne(e => e.StockTakeOrder).WithOne(e => e.Transaction).OnDelete(DeleteBehavior.ClientCascade);
            // builder.Entity<Transaction>().
            //     HasOne(e => e.Product).WithOne(e => e.Transaction).OnDelete(DeleteBehavior.ClientCascade);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.ConfigureWarnings(warn => warn.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
        }
     
    }
}