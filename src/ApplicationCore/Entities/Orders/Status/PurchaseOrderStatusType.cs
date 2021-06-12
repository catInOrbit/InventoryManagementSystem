namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        PQCreated = 1,
        PQSent = 2,
        POCreated = 3,
        POConfirm = 4,
        POSent = 5,
        Done = 6,
        Canceled = -1
    }
}