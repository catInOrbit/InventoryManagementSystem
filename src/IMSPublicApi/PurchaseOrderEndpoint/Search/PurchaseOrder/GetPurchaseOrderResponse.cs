using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetPurchaseOrderResponse : BaseResponse
    {
        public List<ApplicationCore.Entities.Orders.PurchaseOrder> PurchaseOrders { get; set; } =
            new List<ApplicationCore.Entities.Orders.PurchaseOrder>();
    }
}