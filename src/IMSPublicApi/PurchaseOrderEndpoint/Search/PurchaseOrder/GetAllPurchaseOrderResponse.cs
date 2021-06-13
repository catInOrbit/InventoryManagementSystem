using System.Collections.Generic;
using System.Text.Json.Serialization;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.SearchIndex;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PurchaseOrder
{
    public class GetAllPurchaseOrderResponse : BaseResponse
    {

        [JsonIgnore]
        public bool IsDisplayingAll { get; set; }
        
        
        // public List<PurchaseOrderSearchIndex> PurchaseOrderSearchIndices { get; set; } =
        //     new List<PurchaseOrderSearchIndex>();
        public PagingOption<PurchaseOrderSearchIndex> Paging { get; set; }
        public ApplicationCore.Entities.Orders.PurchaseOrder PurchaseOrder { get; set; }
    }
}