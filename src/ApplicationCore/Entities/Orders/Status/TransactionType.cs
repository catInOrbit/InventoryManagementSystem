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
        Supplier = 8,
        Category = 9,
        Location = 10,
        Package = 11,
        Deleted = -1,
        TemplateType = -2,
    }
}