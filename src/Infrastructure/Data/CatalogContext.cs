using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.ApplicationCore.Entities;
using System.Reflection;
using InventoryManagementSystem.ApplicationCore.Entities;

namespace Infrastructure.Data
{

    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        // public DbSet<Basket> Baskets { get; set; }
        public DbSet<Product> CatalogItems { get; set; }
        public DbSet<ProductBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        // public DbSet<Order> Orders { get; set; }
        // public DbSet<OrderItem> OrderItems { get; set; }
        // public DbSet<BasketItem> BasketItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
