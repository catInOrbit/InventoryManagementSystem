using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class PQDisplay
    {

        public PQDisplay()
        {
                
        }
        
        
        public virtual string Id { get; set; }
        public string PriceQuoteOrderNumber { get; set; }
        public float NumberOfProduct { get; set; }
        public string CreatedDate { get; set; }
        public string Deadline { get; set; }
        public string CreatedByName { get; set; }
        public string SupplierName { get; set; }
    }
}