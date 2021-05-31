namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{
    public enum PurchaseOrderStatusType
    {
        Draft = -1,
        Progressing = 1,
        Sent = 2,
        Confirmed = 3,
        Completed = 4,
        Canceled = 5
    }
}