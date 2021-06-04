namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class ReceiveingOrderSearchIndex
    {
        public string ReceiptId { get; set; }
        public string PurchaseOrderId { get; set; }
        public string SupplierName { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}