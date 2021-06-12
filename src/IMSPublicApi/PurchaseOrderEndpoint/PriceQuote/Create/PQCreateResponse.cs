namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote.Create
{
    public class PQCreateResponse : BaseResponse
    {
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrderPQ { get; set; }
    }
}