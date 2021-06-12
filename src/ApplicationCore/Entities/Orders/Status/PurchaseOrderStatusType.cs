namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        RequisitionCreated = 0,
        RequisitionSent = 1,
        PQCreated = 2,
        PQSent = 3,
        POCreated = 4,
        POConfirm = 5,
        POSent = 6,
        Done = 7,
        POCanceled = -1,
        RequisitionCanceled = -2,
    }
}