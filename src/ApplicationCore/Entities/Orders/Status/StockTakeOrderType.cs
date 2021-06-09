namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum StockTakeOrderType
    {
        Progressing = 0,
        Completed = 1,
        Adjusted = 2,
        Cancel = -1
    }
}