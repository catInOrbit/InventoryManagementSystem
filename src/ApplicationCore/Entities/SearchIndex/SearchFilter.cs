namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class GISearchFilter
    {
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string DeliveryMethod { get; set; }
        public string FromDeliveryDate { get; set; }
        public string ToDeliveryDate { get; set; }
    }

    
    public class POSearchFilter
    {
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public string SupplierId { get; set; }
        public string FromTotalOrderPrice { get; set; }
        public string ToTotalOrderPrice { get; set; }
        public string FromDeliveryDate { get; set; }
        public string ToDeliveryDate { get; set; }
        public string FromConfirmedDate { get; set; }
        public string ToConfirmedDate { get; set; }
        
        public string ConfirmedByName { get; set; }
        
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string FromModifiedDate { get; set; }
        public string ToModifiedDate { get; set; }
        public string CreatedByName { get; set; }
        public bool HideMerged { get; set; }
        
        public string[] IgnoreOrderIds { get; set; }
    }
    
    public class ROSearchFilter
    {
        
        public string SupplierName { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string CreatedByName { get; set; }
        
        public string DateSortType { get; set; }

    }
    
    public class STSearchFilter
    {
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string FromDeliveryDate { get; set; }
        public string ToDeliveryDate { get; set; }
    }
    
    public class ProductVariantSearchFilter
    {
        public string Category { get; set; }
        public string Strategy { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string FromModifiedDate { get; set; }
        public string ToModifiedDate { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string FromPrice { get; set; }
        public string ToPrice { get; set; }
        public string Brand { get; set; }
    }
    
    public class PackageSearchFilter
    {
        public string FromImportedDate { get; set; }
        public string ToImportedDate { get; set; }
        public string ProductVariantID { get; set; }
        public string FromTotalPrice { get; set; }
        public string ToTotalPrice { get; set; }
        public string FromPrice { get; set; }
        public string ToPrice { get; set; }
        public string LocationId { get; set; }
        public string FromQuantity { get; set; }
        public string ToQuantity { get; set; }

    }
    
    public class ProductSearchFilter
    {
        public string Category { get; set; }
        public string Strategy { get; set; }
        public string FromCreatedDate { get; set; }
        public string ToCreatedDate { get; set; }
        public string FromModifiedDate { get; set; }
        public string ToModifiedDate { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public string Brand { get; set; }
    }
    
    public class LocationSearchFilter 
    {
        public string FromQuantity { get; set; }
        public string ToQuantity { get; set; }
        public bool NoPackages { get; set; }
    }
}