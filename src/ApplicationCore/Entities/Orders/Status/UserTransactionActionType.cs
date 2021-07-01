namespace InventoryManagementSystem.ApplicationCore.Entities.Orders.Status
{
    public enum UserTransactionActionType
    {
        Create = 0,
        Modify = 1,
        Submit = 2,
        Confirm = 3,
        Reject = 4,
        Delete = -1,
    }
}
