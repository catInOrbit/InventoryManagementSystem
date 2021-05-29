namespace InventoryManagementSystem.ApplicationCore.Entities.Products
{
    public class ProductVariant : BaseEntity
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public string Unit { get; set; }
        public string StorageLocation { get; set; }
        
        public virtual Product Product { get; set; }
    }
}