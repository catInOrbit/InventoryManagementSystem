namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        RequisitionCreated = 0,
        RequisitionMerged = 1,
        PQCreated = 2,
        POCreated = 3,
        POWaitingConfirmation = 4,
        POConfirm = 5,
        Done = 6,
        PQCanceled = -1,
        RequisitionCanceled = -2,
        POCanceled = -3,
    }
}