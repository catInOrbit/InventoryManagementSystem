using System;

namespace InventoryManagementSystem.ApplicationCore.Entities.SearchIndex
{
    public class CategorySearchIndex : BaseSearchIndex
    {
        public override string Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        
        public string TransactionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}