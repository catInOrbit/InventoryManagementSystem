namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum StockTakeOrderType
    {
        Progressing = 0,
        Completed = 1,
        AwaitingAdjustment = 2,
        Adjusted = 3,
        Cancel = -1
    }
}