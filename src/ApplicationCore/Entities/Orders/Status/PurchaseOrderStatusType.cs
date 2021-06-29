namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        RequisitionCreated = 0,
        PQCreated = 1,
        POCreated = 2,
        POWaitingConfirmation = 3,
        POConfirm = 4,
        Done = 5,
        PQCanceled = -1,
        RequisitionCanceled = -2,
        POCanceled = -3,
    }
}