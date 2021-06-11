using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class PQDisplay : BaseEntity
    {

        public PQDisplay()
        {
                
        }
        
        
        public override string Id { get; set; }
        public string PriceQuoteOrderNumber { get; set; }
        public int NumberOfProduct { get; set; }
        public string CreatedDate { get; set; }
        public string Deadline { get; set; }
        public string CreatedByName { get; set; }
        public string SupplierName { get; set; }
    }
}