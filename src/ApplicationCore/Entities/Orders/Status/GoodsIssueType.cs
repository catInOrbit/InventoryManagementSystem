namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum GoodsIssueStatusType
    {
        IssueRequisition = 0,
        Packing = 1,
        Shipping = 2,
        Completed = 3,
        Cancel = -1
    }
}