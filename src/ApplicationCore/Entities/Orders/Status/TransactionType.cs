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
        Product = 6,
        ProductVariant = 7,
        Category = 8,
        Deleted = -1
    }
}