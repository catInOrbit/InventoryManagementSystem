namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GISearchFilter
    {
        public int Status { get; set; }
        public string CreatedById { get; set; }
    }
    
    public class POSearchFilter
    {
        public int Status { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string SupplierId { get; set; }
        public string FromTotalOrderPrice { get; set; }
        public string ToTotalOrderPrice { get; set; }
        public string FromDeliveryDate { get; set; }
        public string ToDeliveryDate { get; set; }
        public string CreatedByName { get; set; }
    }
    
    public class ROSearchFilter
    {
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
    }
}