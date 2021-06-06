namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum PurchaseOrderStatusType
    {
        Created = 1,
        WaitingConfirmation = 2,
        Sent = 3,
        Confirmed = 4,
        GoodsReceiptPhase = 5,
        Done = 6,
        Canceled = -1
    }
}