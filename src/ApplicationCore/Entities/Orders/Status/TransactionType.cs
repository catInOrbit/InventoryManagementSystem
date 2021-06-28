namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum TransactionType
    {
        Requisition = 0,
        PriceQuote = 1,
        Purchase = 2,
        GoodsReceipt= 3,
        GoodsIssue= 4,
        StockTake= 5,
        NewProduct = 6,
        Deleted = -1
    }
}