namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        RequisitionCreated = 0,
        RequisitionSent = 1,
        PQCreated = 2,
        PQSent = 3,
        POCreated = 4,
        POWaitingConfirmation = 5,
        POConfirm = 6,
        POSent = 7,
        Done = 8,
        PQCanceled = -1,
        RequisitionCanceled = -2,
        POCanceled = -3,
    }
}