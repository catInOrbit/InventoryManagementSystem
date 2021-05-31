namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public enum PurchaseOrderStatusType
    {
        Created = -1,
        WaitingConfirmation = 1,
        Sent = 2,
        Confirmed = 3,
        GoodsReceiptPhase = 4,
        Canceled = 5
    }
}