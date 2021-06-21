namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public abstract class GeneralFilter
    {
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string FromModifiedDate { get; set; }
        public string ToModifiedDate { get; set; }
        public string CreatedByName { get; set; }

    }
    
    public class GISearchFilter
    {
        public int Status { get; set; }
        public string CreatedById { get; set; }
    }
    
    public class POSearchFilter : GeneralFilter
    {
        public int Status { get; set; }
        public string SupplierId { get; set; }
        public string FromTotalOrderPrice { get; set; }
        public string ToTotalOrderPrice { get; set; }
        public string FromDeliveryDate { get; set; }
        public string ToDeliveryDate { get; set; }
    }
    
    public class ROSearchFilter : GeneralFilter
    {
        public string SupplierName { get; set; }
    }
    
    public class RequisitionFilter : GeneralFilter
    {
        public int Status { get; set; }
        public string SubmitBy { get; set; }

    }
}