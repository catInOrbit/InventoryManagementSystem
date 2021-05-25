namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; set; }
        public string SalePersonName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string SupplyType { get; set; }
        
    }
}