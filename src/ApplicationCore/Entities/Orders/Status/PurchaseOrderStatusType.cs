namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        RequisitionCreated = 0,
        RequisitionMerged = 1,
        Requisition = 2,
        PriceQuote = 3,
        PurchaseOrder = 4,
        POWaitingConfirmation = 5,
        POConfirm = 6,
        Done = 7,
        PQCanceled = -1,
        RequisitionCanceled = -2,
        POCanceled = -3,
    }
}