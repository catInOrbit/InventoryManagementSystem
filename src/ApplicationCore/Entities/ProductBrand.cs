namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class ProductBrand : BaseEntity
    {
        public string Brand { get; private set; }
        public ProductBrand(string brand)
        {
            Brand = brand;
        }
    }
}
