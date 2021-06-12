using InventoryManagementSystem.ApplicationCore.Entities.Orders;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.PriceQuote
{
    public class PQSubmitRequest : BaseRequest
    {
        public string OrderNumber { get; set; }
        public string[] To { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        

    }
}