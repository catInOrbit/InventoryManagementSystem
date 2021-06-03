using System;

namespace InventoryManagementSystem.PublicApi.PurchaseOrderEndpoint.Search.PriceQuote
{
    public class PQDisplay
    {

        public PQDisplay()
        {
                
        }
        
        
        public virtual string Id { get; set; }
        public string PriceQuoteOrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime Deadline { get; set; }
        public string CreatedByName { get; set; }

    }
}