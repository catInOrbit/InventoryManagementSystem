using System.Collections.Generic;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search
{
    public class GetPurchaseOrderResponse : BaseResponse
    {
        public List<ApplicationCore.Entities.Orders.PurchaseOrder> PurchaseOrders { get; set; }
    }
}