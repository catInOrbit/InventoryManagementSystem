using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetAllPurchaseOrderResponse : BaseResponse
    {
        
        public List<PurchaseOrderSearchIndex> PurchaseOrderSearchIndices { get; set; } =
            new List<PurchaseOrderSearchIndex>();

        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
}