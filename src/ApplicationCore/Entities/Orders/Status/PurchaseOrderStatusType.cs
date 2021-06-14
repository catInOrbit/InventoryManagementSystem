namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        RequisitionCreated = 0,
        PQCreated = 1,
        PQSent = 2,
        POCreated = 3,
        POConfirm = 4,
        POSent = 5, 
        Done = 6,
        PQCanceled = -1,
        RequisitionCanceled = -2,
        POCanceled = -3,
    }
}